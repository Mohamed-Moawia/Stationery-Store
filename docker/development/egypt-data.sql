-- docker/development/egypt-data.sql
-- Egyptian-specific initial data
INSERT INTO catalog.units (id, code, name_ar, name_en, conversion_factor) VALUES
(uuid_generate_v4(), 'PCS', 'قطعة', 'Piece', 1.0),
(uuid_generate_v4(), 'BOX', 'علبة', 'Box', 12.0),
(uuid_generate_v4(), 'PKT', 'حزمة', 'Packet', 10.0),
(uuid_generate_v4(), 'REAM', 'رزمة', 'Ream', 500.0);

-- Egyptian VAT rates (14% standard as of 2024)
INSERT INTO catalog.vat_rates (id, name_ar, name_en, rate, eta_tax_type_code) VALUES
(uuid_generate_v4(), 'ضريبة قياسية', 'Standard VAT', 14.00, 'T1'),
(uuid_generate_v4(), 'معفى', 'Exempt', 0.00, 'T2'),
(uuid_generate_v4(), 'صفر', 'Zero Rated', 0.00, 'T3'),
(uuid_generate_v4(), 'خاص', 'Special Rate', 5.00, 'T4');

-- Egyptian cities for branches
INSERT INTO public.egyptian_cities (id, name_ar, name_en, governorate_ar, governorate_en) VALUES
(uuid_generate_v4(), 'القاهرة', 'Cairo', 'القاهرة', 'Cairo'),
(uuid_generate_v4(), 'الجيزة', 'Giza', 'الجيزة', 'Giza'),
(uuid_generate_v4(), 'الإسكندرية', 'Alexandria', 'الإسكندرية', 'Alexandria'),
(uuid_generate_v4(), 'بورسعيد', 'Port Said', 'بورسعيد', 'Port Said');

-- Sample Egyptian tax activities (النشاط الضريبي)
INSERT INTO public.tax_activities (id, code, description_ar, description_en) VALUES
(uuid_generate_v4(), '47891', 'تجارة الجملة والتجزئة في الأدوات المكتبية والقرطاسية', 'Wholesale and retail trade in office supplies and stationery'),
(uuid_generate_v4(), '47912', 'بيع الكتب والصحف والمجلات', 'Sale of books, newspapers and magazines');
