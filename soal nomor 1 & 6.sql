--create Database
CREATE DATABASE pengajuan_kredit_db;

--create Table
CREATE TABLE pengajuan_kredit (
    id UUID PRIMARY KEY,
    plafon NUMERIC NOT NULL,
    bunga DECIMAL(5,2) NOT NULL CHECK (bunga >= 0 AND bunga <= 100),
    tenor INTEGER NOT NULL CHECK (tenor > 0),
    angsuran NUMERIC NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

--indexing
CREATE INDEX idx_plafon ON pengajuan_kredit(plafon);
CREATE INDEX idx_tenor ON pengajuan_kredit(tenor);

--Mencari Pengajuan Kredit dengan Tenor paling panjang dan Plafon Tertinggi
SELECT * FROM pengajuan_kredit ORDER BY tenor DESC, plafon desc LIMIT 1;

--Menghitung rata-rata bunga dari semua pengajuan kredit:
SELECT AVG(bunga) AS rata_rata_bunga FROM pengajuan_kredit;



--soal nomor 6
6.1 
--Optimasi Query
--1. Selalu gunakan .AsNoTracking() untuk query read-only di Entity Framework.
--2. Pastikan kolom-kolom yang sering digunakan sebagai filter (misalnya Id, Tenor, Plafon) memiliki index di PostgreSQL.
--3. Semua operasi database menggunakan async/await agar tidak blocking thread pool
--4. Pastikan pooling aktif di connection string 
--5. Pagination (Hindari mengambil semua data dalam satu request)

--Upgrage pola Event-Driven Architecture
--menurut saya pribadi untuk pilihan terbaik untuk mengelola data ribuan user perhari menggunakan architecture event-driven pilihan terbaik
--kenapa karena setiap service berkomunikasi melalui message broker itu sudah di handle dengan message broker dengan request atau throughput 
--100rb message per detiknya (https://www.rabbitmq.com/blog/2023/05/17/rabbitmq-3.12-performance-improvements) referensi yang saya gunakan 
-- untuk handle 100rb data per detiknya dan message broker sudah ada fitur High scalability jadi aman saat ada peningkatan proses secara signifikan
-- pastinya pola ini di gabung dengan arcitecture microservice jg jadi sertiap proses service di pisah permodul jika ada satu service yang hts cukup tinggi
-- di sana di fokuskan untuk peningkatan nodenya jadi semua tergantung kebutuhan 
6.2
--Saya akan menggunakan Redis untuk caching data yang tidak sering berubah seperti referensi dan lookup tabel

6.3
-- Service-service dipisah 
-- Gateway Routing untuk mengatur routing antar service
-- komunikasi di bagi jadi dua satu menggunakan restapi satu menggunakan message broker untuk data yang besar
-- scalability setiap service sesuai kebutuhan (Pembagian node service)



