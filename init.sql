-- ============================================================
-- SwiftParcel Case Management System
-- Legacy Database (migrated from MS Access, as-is)
-- ============================================================

DROP TABLE IF EXISTS cases;
DROP TABLE IF EXISTS case_notes;
DROP TABLE IF EXISTS parcels;
DROP TABLE IF EXISTS customers;
DROP TABLE IF EXISTS handlers;
DROP TABLE IF EXISTS system_config;
DROP TABLE IF EXISTS email_templates;
DROP TABLE IF EXISTS sla_rules;
DROP TABLE IF EXISTS status_workflow;
DROP TABLE IF EXISTS auto_assignment_rules;
DROP TABLE IF EXISTS holidays;
DROP TABLE IF EXISTS regions;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS roles;
DROP TABLE IF EXISTS user_permissions;
DROP TABLE IF EXISTS audit_log;

CREATE TABLE handlers (
    id VARCHAR(50),
    full_name VARCHAR(255),
    email VARCHAR(255),
    department VARCHAR(100),
    region VARCHAR(100),
    is_active VARCHAR(10),
    phone VARCHAR(100),
    hire_date VARCHAR(50),
    max_cases VARCHAR(10)
);

INSERT INTO handlers (id, full_name, email, department, region, is_active, phone, hire_date, max_cases) VALUES
('1', 'Anna Kovacs', 'anna.kovacs@swiftparcel.com', 'Customer Support', 'Budapest', 'yes', '+36 30 123 4567', '2019-03-15', '20'),
('2', 'Tomas Novak', 'tomas.novak@swiftparcel.com', 'Customer Support', 'Prague', 'yes', '+420 777 888 999', '15/06/2020', '25'),
('3', 'Maria Schmidt', 'maria.schmidt@swiftparcel.com', 'Escalations', 'Vienna', 'true', '0043 699 1234567', '2021-01-10', '15'),
('4', 'Jakub Wiśniewski', 'jakub.w@swiftparcel.com', 'Customer Support', 'Warsaw', '1', '+48 512 345 678', '2022/04/01', '20'),
('5', 'Peter Horvath', 'peter.horvath@swiftparcel.com', 'Customer Support', 'Budapest', 'no', '+36 70 987 6543', '2018-11-20', '0'),
('6', 'Elena Dvorak', 'elena.dvorak@swiftparcel.com', 'Investigations', 'Prague', 'yes', '+420 608 111 222', '2020-08-08', '10');

CREATE TABLE customers (
    id VARCHAR(50),
    name VARCHAR(255),
    email VARCHAR(255),
    phone VARCHAR(100),
    address VARCHAR(500),
    registered_date VARCHAR(50),
    vip VARCHAR(20),
    notes VARCHAR(1000)
);

INSERT INTO customers (id, name, email, phone, address, registered_date, vip, notes) VALUES
('C001', 'Janos Szabo', 'janos.szabo@gmail.com', '+36201234567', 'Budapest, Váci út 15, 1052', '2020-01-15', 'no', ''),
('C002', 'Petra Müller', 'petra.mueller@outlook.com', '+43 664 555 1234', 'Wien, Mariahilfer Straße 42, 1060', '15/03/2019', 'yes', 'Key account - handle with priority'),
('C003', 'Karel Svoboda', 'karel.svoboda@seznam.cz', '00420 723 456 789', 'Praha 1, Národní 10, 110 00', '2021-06-30', 'no', NULL),
('C004', 'Ewa Kowalska', 'ewa.kowalska@wp.pl', '+48 600 123 456', 'Warszawa, ul. Marszałkowska 1, 00-001', '2022/02/28', 'false', ''),
('C005', 'Hans Weber', 'hans.weber@gmx.at', '+43 676 999 8888', 'Graz, Herrengasse 7, 8010', '01-12-2020', 'yes', 'Frequent shipper, business account'),
('C006', 'János Szabó', 'janos.szabo@gmail.com', '+36-20-123-4567', 'Budapest, Vaci ut 15., H-1052', '2020-01-15', 'no', 'same person as C001?'),
('C007', 'Lukas Bauer', 'l.bauer@gmail.com', '+43 660 777 3333', 'Linz, Landstrasse 20, 4020', '2023-03-01', 'n', '');

CREATE TABLE parcels (
    id VARCHAR(50),
    tracking_number VARCHAR(100),
    sender_name VARCHAR(255),
    sender_address VARCHAR(500),
    recipient_name VARCHAR(255),
    recipient_address VARCHAR(500),
    weight VARCHAR(50),
    dimensions VARCHAR(100),
    status VARCHAR(100),
    created_date VARCHAR(50),
    delivered_date VARCHAR(50),
    service_type VARCHAR(100),
    declared_value VARCHAR(50),
    customer_id VARCHAR(50)
);

INSERT INTO parcels (id, tracking_number, sender_name, sender_address, recipient_name, recipient_address, weight, dimensions, status, created_date, delivered_date, service_type, declared_value, customer_id) VALUES
('P001', 'SP-20230101', 'Janos Szabo', 'Budapest, Váci út 15, 1052', 'Maria Kiss', 'Debrecen, Piac utca 5, 4024', '2.5 kg', '30x20x15', 'delivered', '2024-01-10', '2024-01-12', 'standard', '50 EUR', 'C001'),
('P002', 'SP-20230102', 'Petra Müller', 'Wien, Mariahilfer Straße 42', 'Klaus Fischer', 'Salzburg, Getreidegasse 3, 5020', '1.2kg', '20x15x10 cm', 'delivered', '12/01/2024', '14/01/2024', 'express', '120EUR', 'C002'),
('P003', 'SP-20230103', 'Karel Svoboda', 'Praha 1, Národní 10', 'Jan Novotný', 'Brno, Masarykova 15, 602 00', '0.8', '15x10x10', 'lost', '2024-01-15', '', 'standard', '30', 'C003'),
('P004', 'SP-20230104', 'Ewa Kowalska', 'Warszawa, ul. Marszałkowska 1', 'Adam Nowak', 'Kraków, Rynek Główny 1, 30-001', '5', '40x30x30', 'damaged', '2024-01-18', '2024-01-22', 'standard', '200 EUR', 'C004'),
('P005', 'SP-20230105', 'Hans Weber', 'Graz, Herrengasse 7', 'Petra Müller', 'Wien, Mariahilfer Straße 42', '3.1 kg', '35x25x20', 'in_transit', '2024-02-01', NULL, 'express', '€ 85', 'C005'),
('P006', 'SP-20230106', 'Janos Szabo', 'Budapest, Vaci ut 15', 'Tamas Nagy', 'Szeged, Kárász u. 1, 6720', '1.0 kg', '20x20x20', 'delivered', '2024-02-05', '07-02-2024', 'standard', '25 EUR', 'C001'),
('P007', 'SP-20230107', 'Lukas Bauer', 'Linz, Landstrasse 20, 4020', 'Max Huber', 'Wien, Stephansplatz 1, 1010', '0.5 kg', '10x10x5', 'delivered', '2024-02-10', '2024-02-11', 'same_day', '15 EUR', 'C007'),
('P008', 'SP20230108', 'Petra Mueller', 'Wien, Mariahilferstr. 42', 'Hans Weber', 'Graz, Herrengasse 7, 8010', '2.0', '25x20x15', 'in_transit', '2024-02-15', NULL, 'standard', '60', 'C002'),
('P009', 'SP-20230109', 'Ewa Kowalska', 'Warsaw, Marszalkowska 1', 'Piotr Zielinski', 'Gdańsk, Długa 10, 80-001', '4.5 kg', '45x35x25', 'delivered', '2024-02-20', '2024-02-23', 'standard', '150 EUR', 'C004');

CREATE TABLE cases (
    id VARCHAR(50),
    case_number VARCHAR(50),
    title VARCHAR(500),
    description VARCHAR(2000),
    case_type VARCHAR(100),
    status VARCHAR(100),
    priority VARCHAR(50),
    customer_name VARCHAR(255),
    customer_email VARCHAR(255),
    customer_phone VARCHAR(100),
    handler_id VARCHAR(50),
    handler_name VARCHAR(255),
    parcel_tracking_numbers VARCHAR(500),
    created_date VARCHAR(50),
    updated_date VARCHAR(50),
    resolved_date VARCHAR(50),
    sla_deadline VARCHAR(50),
    region VARCHAR(100),
    channel VARCHAR(100),
    tags VARCHAR(500),
    is_escalated VARCHAR(20),
    escalated_to VARCHAR(100),
    resolution VARCHAR(1000),
    satisfaction_score VARCHAR(20)
);

INSERT INTO cases (id, case_number, title, description, case_type, status, priority, customer_name, customer_email, customer_phone, handler_id, handler_name, parcel_tracking_numbers, created_date, updated_date, resolved_date, sla_deadline, region, channel, tags, is_escalated, escalated_to, resolution, satisfaction_score) VALUES
('1', 'CASE-2024-001', 'Lost parcel - no tracking update for 5 days', 'Customer reports parcel SP-20230103 has not been delivered and tracking shows no update since Jan 16.', 'LOST', 'In Progress', 'high', 'Karel Svoboda', 'karel.svoboda@seznam.cz', '00420 723 456 789', '2', 'Tomas Novak', 'SP-20230103', '2024-01-20', '2024-01-22', '', '2024-01-22 18:00', 'Prague', 'phone', 'lost,investigation,priority', 'no', '', '', ''),
('2', 'CASE-2024-002', 'Damaged package on arrival', 'Package SP-20230104 arrived with visible damage. Contents (electronics) broken. Customer requests full refund.', 'DAMAGED', 'Awaiting Customer', 'high', 'Ewa Kowalska', 'ewa.kowalska@wp.pl', '+48 600 123 456', '4', 'Jakub Wiśniewski', 'SP-20230104', '2024-01-23', '24/01/2024', NULL, '2024-01-25', 'Warsaw', 'email', 'damaged,refund,electronics', 'no', NULL, NULL, NULL),
('3', 'CASE-2024-003', 'Delivery to wrong address', 'Parcel SP-20230101 was delivered to neighbor. Customer found it 2 days later, opened.', 'WRONG_ADDRESS', 'Resolved', 'medium', 'Maria Kiss', 'not provided', '', '1', 'Anna Kovacs', 'SP-20230101', '2024-01-14', '2024-01-15', '15-01-2024', '2024-01-16', 'Budapest', 'phone', 'wrong_address,neighbor', 'no', '', 'Parcel was confirmed received by correct recipient. Apology sent.', '4'),
('4', 'CASE-2024-004', 'Billing dispute - double charged', 'Customer claims they were charged twice for parcel SP-20230102. Amount: 120 EUR x2.', 'BILLING', 'Resolved', 'medium', 'Petra Müller', 'petra.mueller@outlook.com', '+43 664 555 1234', '3', 'Maria Schmidt', 'SP-20230102', '2024-01-20', '2024-01-21', '2024-01-21', '2024-01-22 18:00', 'Vienna', 'email', 'billing,duplicate_charge,vip', 'no', '', 'Confirmed double charge. Refund of 120 EUR processed.', '5'),
('5', 'CASE-2024-005', 'Late delivery - express parcel', 'Express parcel SP-20230105 not delivered within promised 24h window. Still in transit after 3 days.', 'DELAYED', 'Open', 'high', 'Hans Weber', 'hans.weber@gmx.at', '+43 676 999 8888', '3', 'Maria Schmidt', 'SP-20230105', '2024-02-04', '2024-02-04', '', '2024-02-06', 'Vienna', 'phone', 'delayed,express,sla_breach,vip', 'yes', 'Maria Schmidt', '', ''),
('6', 'CASE-2024-006', 'Multiple parcels lost in same shipment', 'Customer reports both SP-20230106 and SP-20230108 are missing. Tracking shows delivered but customer says never received.', 'LOST', 'In Progress', 'high', 'Janos Szabo', 'janos.szabo@gmail.com', '+36201234567', '1', 'Anna Kovacs', 'SP-20230106, SP-20230108', '2024-02-10', '2024-02-12', NULL, '12/02/2024 18:00', 'Budapest', 'phone', 'lost,multiple_parcels,investigation', 'no', '', '', ''),
('7', 'CASE-2024-007', 'Damaged contents - fragile items', 'Wine bottles broken inside parcel SP-20230109. Package exterior looks fine but contents destroyed.', 'DAMAGED', 'Closed', 'low', 'Piotr Zielinski', 'piotr.z@onet.pl', '+48 500 111 222', '4', 'Jakub Wisniewski', 'SP-20230109', '2024-02-24', '2024-02-28', '28/02/2024', '2024-02-26', 'Warsaw', 'email', 'damaged,fragile,insurance', 'no', '', 'Insurance claim filed. Customer compensated 150 EUR.', '3'),
('8', 'CASE-2024-008', 'Parcel tracking not updating', 'Tracking for SP-20230108 stuck on "in transit" for 5 days. Customer concerned.', 'DELAYED', 'Open', 'medium', 'Petra Mueller', 'petra.mueller@outlook.com', '+43 664 5551234', '2', 'Tomas Novak', 'SP-20230108', '2024-02-20', '2024-02-20', NULL, '2024-02-22', 'Prague', 'chat', 'tracking,stuck,delayed', 'no', NULL, NULL, NULL),
('9', 'CASE-2024-009', 'Request for same-day delivery compensation', 'Same-day delivery SP-20230107 arrived 2 hours late. Customer wants partial refund.', 'BILLING', 'Resolved', 'low', 'Lukas Bauer', 'l.bauer@gmail.com', '+43 660 777 3333', '3', 'Maria Schmidt', 'SP-20230107', '2024-02-12', '2024-02-13', '2024-02-13', '2024-02-14 18:00', 'Vienna', 'email', 'billing,compensation,same_day', 'no', '', '50% refund of delivery fee processed. 7.50 EUR credited.', '4'),
('10', 'CASE-2024-010', 'Lost parcel - international', 'Parcel SP-20230103 status still shows lost after 3 weeks. Customer demanding escalation.', 'LOST', 'Escalated', 'critical', 'Karel Svoboda', 'karel.svoboda@seznam.cz', '00420723456789', '6', 'Elena Dvorak', 'SP-20230103', '2024-02-05', '2024-02-10', '', '2024-02-07', 'Prague', 'phone', 'lost,escalated,international,long_running', 'yes', 'Elena Dvorak', '', ''),
('11', 'CASE-2024-011', 'Wrong item in package', 'Customer received someone elses parcel. Tracking number SP-20230106 shows delivered correctly but contents are wrong.', 'OTHER', 'In Progress', 'medium', 'Tamas Nagy', 'tamas.nagy@freemail.hu', '+36 30 555 9999', '1', 'Anna Kovács', 'SP-20230106', '2024-02-08', '2024-02-09', NULL, '2024-02-10', 'Budapest', 'phone', 'wrong_item,swap,investigation', 'no', NULL, NULL, NULL);

CREATE TABLE case_notes (
    id VARCHAR(50),
    case_id VARCHAR(50),
    case_number VARCHAR(100),
    author VARCHAR(255),
    author_email VARCHAR(255),
    note_text VARCHAR(4000),
    created_date VARCHAR(50),
    is_internal VARCHAR(20),
    attachment VARCHAR(500)
);

INSERT INTO case_notes (id, case_id, case_number, author, author_email, note_text, created_date, is_internal, attachment) VALUES
('N001', '1', 'CASE-2024-001', 'Tomas Novak', 'tomas.novak@swiftparcel.com', 'Contacted warehouse in Brno. They confirm parcel left on Jan 16. Checking with delivery partner.', '2024-01-21 09:30', 'yes', ''),
('N002', '1', 'CASE-2024-001', 'Tomas Novak', 'tomas.novak@swiftparcel.com', 'Delivery partner says parcel was handed to subcontractor. Awaiting their response.', '2024-01-22 14:15', 'yes', NULL),
('N003', '2', 'CASE-2024-002', 'Jakub Wiśniewski', 'jakub.w@swiftparcel.com', 'Asked customer to send photos of damaged package and contents.', '2024-01-23 16:00', 'no', ''),
('N004', '2', 'CASE-2024-002', 'Jakub Wiśniewski', 'jakub.w@swiftparcel.com', 'Photos received. Visible crush damage on box. Electronics shattered. Forwarding to claims dept.', '24/01/2024 10:30', 'yes', 'damage_photos_case002.zip'),
('N005', '3', 'CASE-2024-003', 'Anna Kovacs', 'anna.kovacs@swiftparcel.com', 'Called customer Maria Kiss. She confirmed she found the parcel at neighbors door. Contents intact.', '2024-01-15 11:00', 'no', NULL),
('N006', '4', 'CASE-2024-004', 'Maria Schmidt', 'maria.schmidt@swiftparcel.com', 'Checked payment system. Confirmed duplicate charge on Jan 12. Processing refund.', '2024-01-21 08:45', 'true', ''),
('N007', '5', 'CASE-2024-005', 'Maria Schmidt', 'maria.schmidt@swiftparcel.com', 'VIP customer - escalating immediately. Express SLA already breached.', '2024-02-04 09:00', '1', ''),
('N008', '6', 'CASE-2024-006', 'Anna Kovacs', 'anna.kovacs@swiftparcel.com', 'Note: SP-20230108 belongs to customer C002 (Petra Müller), not C001 (Janos Szabo). Possible mix-up in case creation. Investigating both parcels.', '2024-02-11 10:00', 'yes', ''),
('N009', '6', 'CASE-2024-006', 'Anna Kovacs', 'anna.kovacs@swiftparcel.com', 'SP-20230106 confirmed delivered to correct address per GPS. Customer insists not received. Scheduling driver interview.', '2024-02-12 15:30', 'yes', NULL),
('N010', '7', 'CASE-2024-007', 'Jakub Wisniewski', 'jakub.w@swiftparcel.com', 'Fragile sticker was on package but no special handling was done. Filing insurance claim with carrier.', '2024-02-25 09:00', 'internal', ''),
('N011', '7', 'CASE-2024-007', 'Jakub Wisniewski', 'jakub.w@swiftparcel.com', 'Insurance approved. Compensation of 150 EUR will be credited to customer account within 5 business days.', '2024-02-27 16:45', 'no', 'insurance_approval_007.pdf'),
('N012', '10', 'CASE-2024-010', 'Tomas Novak', 'tomas.novak@swiftparcel.com', 'Original case CASE-2024-001 still unresolved after 2 weeks. Customer called again demanding escalation. Transferring to investigations team.', '2024-02-05 11:00', 'yes', ''),
('N013', '10', 'CASE-2024-010', 'Elena Dvorak', 'elena.dvorak@swiftparcel.com', 'Taking over from Tomas. Opening formal investigation. Contacting all parties in delivery chain.', '2024-02-06 08:30', 'yes', NULL),
('N014', '10', 'CASE-2024-010', 'Elena Dvorak', 'elena.dvorak@swiftparcel.com', 'Subcontractor admits parcel may have been misrouted to Germany. Tracing internationally now.', '2024-02-10 14:00', 'yes', ''),
('N015', '11', 'CASE-2024-011', 'Anna Kovács', 'anna.kovacs@swiftparcel.com', 'Customer Tamas Nagy received parcel with womens clothing instead of his electronics order. Clearly a swap at sorting facility.', '2024-02-08 14:30', 'no', ''),
('N016', '9', 'CASE-2024-009', 'Maria Schmidt', 'maria.schmidt@swiftparcel.com', 'Same-day SLA is 6 hours. Delivered in 8 hours. Approved 50% delivery fee refund per policy.', '2024-02-13 09:15', 'yes', '');


CREATE TABLE system_config (
    id VARCHAR(50),
    config_key VARCHAR(255),
    config_value VARCHAR(4000),
    description VARCHAR(1000),
    updated_by VARCHAR(255),
    updated_date VARCHAR(50)
);

INSERT INTO system_config (id, config_key, config_value, description, updated_by, updated_date) VALUES
('SC01', 'default_language', 'EN', 'Default language for new cases', 'admin', '2023-01-01'),
('SC02', 'pagination_size', '25', 'Number of items per page', 'admin', '2023-01-01'),
('SC03', 'max_attachment_size_mb', '10', 'Maximum attachment size in MB', 'admin', '2023-01-01'),
('SC04', 'auto_close_days', '7', 'Days after resolved before auto-closing', 'admin', '2023-06-15'),
('SC05', 'maintenance_mode', 'false', 'Set to true to enable maintenance mode', 'admin', '2024-01-01'),
('SC06', 'supported_languages', 'HU,DE,CZ,PL,EN', 'Comma-separated list of supported languages', 'admin', '2023-01-01'),
('SC07', 'case_number_format', 'CASE-{YYYY}-{SEQ:3}', 'Format pattern for case numbers', 'admin', '2023-01-01'),
('SC08', 'max_attachment_size_mb', '15', 'Updated max attachment size', 'maria.schmidt', '2024-02-01'),
('SC09', 'smtp_config', '{"host":"mail.swiftparcel.com","port":"587","user":"noreply@swiftparcel.com","password":"Sw1ftM@il2023!","tls":"true"}', 'SMTP configuration as JSON', 'admin', '2023-01-01'),
('SC10', 'escalation_email_recipients', 'maria.schmidt@swiftparcel.com, elena.dvorak@swiftparcel.com, mgmt-team@swiftparcel.com', 'Who gets escalation emails', 'admin', '2023-09-01'),
('SC11', 'allowed_file_types', '.jpg,.png,.pdf,.zip,.doc,.docx,.xls,.xlsx', 'Allowed attachment file extensions', 'admin', '2023-01-01'),
('SC12', 'vip_auto_priority', 'high', 'Priority to assign to VIP customer cases', 'admin', '2023-03-01'),
('SC13', 'enable_auto_assignment', 'yes', 'Whether auto-assignment is enabled', 'admin', '2024-01-15'),
('SC14', 'enable_auto_assignment', 'true', 'Enable auto-assignment feature', 'tomas.novak', '2024-02-10'),
('SC15', 'dashboard_widgets', '["open_cases_count","sla_breaches","my_cases","recent_activity","cases_by_type_chart"]', 'Dashboard widget configuration as JSON array', 'admin', '2023-06-01'),
('SC16', 'session_timeout_minutes', '30', 'User session timeout', 'admin', '2023-01-01'),
('SC17', 'case_types', 'LOST|DAMAGED|DELAYED|WRONG_ADDRESS|BILLING|OTHER', 'Available case types (pipe-separated)', 'admin', '2023-01-01'),
('SC18', 'priority_levels', '{"critical":1,"high":2,"medium":3,"low":4}', 'Priority levels with sort order', 'admin', '2023-01-01');

CREATE TABLE email_templates (
    id VARCHAR(50),
    template_name VARCHAR(255),
    language VARCHAR(10),
    region VARCHAR(100),
    subject VARCHAR(500),
    body VARCHAR(4000),
    is_active VARCHAR(10),
    created_by VARCHAR(255),
    created_date VARCHAR(50)
);

INSERT INTO email_templates (id, template_name, language, region, subject, body, is_active, created_by, created_date) VALUES
('ET01', 'case_created', 'EN', '', 'Your case {{case_number}} has been created', '<html><body><h1>Hello {{customer_name}},</h1><p>We have received your inquiry regarding tracking number {{tracking_number}}.</p><p>Your case reference is: <b>{{case_number}}</b></p><p>We will get back to you within {{sla_hours}} hours.</p><p>Best regards,<br>SwiftParcel Customer Support</p></body></html>', 'yes', 'admin', '2023-01-01'),
('ET02', 'case_created', 'DE', '', 'Ihr Fall {{case_number}} wurde erstellt', '<html><body><h1>Hallo {{customer_name}},</h1><p>Wir haben Ihre Anfrage bezüglich der Sendungsnummer {{tracking_number}} erhalten.</p><p>Ihre Fallreferenz lautet: <b>{{case_number}}</b></p><p>Wir melden uns innerhalb von {{sla_hours}} Stunden.</p><p>Mit freundlichen Grüßen,<br>SwiftParcel Kundenservice</p></body></html>', 'yes', 'admin', '2023-01-01'),
('ET03', 'case_created', 'HU', '', 'Az Ön ügye ({{case_number}}) létrejött', '<html><body><h1>Kedves {{customer_name}}!</h1><p>Megkaptuk a(z) {{tracking_number}} számú csomagjával kapcsolatos megkeresését.</p><p>Az ügy hivatkozási száma: <b>{{case_number}}</b></p><p>{{sla_hours}} órán belül jelentkezünk.</p><p>Üdvözlettel,<br>SwiftParcel Ügyfélszolgálat</p></body></html>', 'yes', 'admin', '2023-01-01'),
('ET04', 'case_created', 'CZ', '', 'Váš případ {{case_number}} byl vytvořen', '<html><body><h1>Dobrý den {{customer_name}},</h1><p>Obdrželi jsme Váš dotaz ohledně zásilky {{tracking_number}}.</p><p>Referenční číslo: <b>{{case_number}}</b></p><p>Ozveme se Vám do {{sla_hours}} hodin.</p></body></html>', 'true', 'admin', '2023-01-01'),
('ET05', 'case_created', 'PL', '', 'Twoja sprawa {{case_number}} została utworzona', '<html><body><h1>Witaj [customer_name],</h1><p>Otrzymaliśmy Twoje zapytanie dotyczące przesyłki [tracking_number].</p><p>Numer referencyjny: <b>[case_number]</b></p><p>Odpowiemy w ciągu [sla_hours] godzin.</p></body></html>', '1', 'admin', '2023-01-01'),
('ET06', 'case_resolved', 'EN', '', 'Your case {{case_number}} has been resolved', '<html><body><h1>Hello {{customer_name}},</h1><p>Your case <b>{{case_number}}</b> has been resolved.</p><p><b>Resolution:</b> {{resolution_text}}</p><p>If you are not satisfied, please reply to this email within 7 days.</p><p>Best regards,<br>SwiftParcel</p></body></html>', 'yes', 'admin', '2023-01-01'),
('ET07', 'case_resolved', 'DE', '', 'Ihr Fall {{case_number}} wurde gelöst', '<html><body><h1>Hallo {{customer_name}},</h1><p>Ihr Fall <b>{{case_number}}</b> wurde gelöst.</p><p><b>Lösung:</b> {{resolution_text}}</p><p>Falls Sie nicht zufrieden sind, antworten Sie bitte innerhalb von 7 Tagen auf diese E-Mail.</p></body></html>', 'yes', 'admin', '2023-01-01'),
('ET08', 'sla_warning', 'EN', '', 'SLA WARNING: Case {{case_number}} approaching deadline', 'PLAIN TEXT: Case {{case_number}} (type: {{case_type}}) is approaching its SLA deadline. Current handler: {{handler_name}}. Deadline: {{sla_deadline}}. Please take action immediately.', 'yes', 'admin', '2023-01-01'),
('ET09', 'sla_breach', 'EN', '', 'SLA BREACH: Case {{case_number}} has exceeded deadline', '<html><body style="color:red"><h1>SLA BREACH ALERT</h1><p>Case <b>{{case_number}}</b> has exceeded its SLA deadline.</p><p>Type: {{case_type}}<br>Handler: {{handler_name}}<br>Deadline was: {{sla_deadline}}<br>Hours overdue: {{hours_overdue}}</p></body></html>', 'yes', 'admin', '2023-01-01'),
('ET10', 'case_assigned', 'EN', '', 'Case {{case_number}} assigned to you', '<html><body><p>Hi {{handler_name}},</p><p>Case <b>{{case_number}}</b> has been assigned to you.</p><p>Customer: {{customer_name}}<br>Type: {{case_type}}<br>Priority: {{priority}}</p><p>SLA deadline: {{sla_deadline}}</p></body></html>', 'yes', 'admin', '2023-01-01'),
('ET11', 'case_created', 'EN', 'Vienna', 'Your case {{case_number}} has been created - AT Support', '<html><body><h1>Hello {{customer_name}},</h1><p>Thank you for contacting SwiftParcel Austria.</p><p>Your tracking number: {{tracking_number}}</p><p>Case: <b>{{case_number}}</b></p><p>We aim to respond within {{sla_hours}} hours.</p><p>Kind regards,<br>SwiftParcel Österreich</p></body></html>', 'yes', 'admin', '2023-06-01'),
('ET12', 'case_escalated', 'EN', '', 'Case $case_number$ has been escalated', '<html><body><p>Case <b>$case_number$</b> has been escalated.</p><p>Previous handler: $previous_handler$<br>New handler: $new_handler$<br>Reason: $escalation_reason$</p></body></html>', 'yes', 'admin', '2023-09-01'),
('ET13', 'weekly_report', 'EN', '', 'Weekly Case Report - {{week_number}}', 'This template is no longer used since we switched to the dashboard.', 'no', 'admin', '2023-01-01'),
('ET14', 'case_resolved', 'HU', '', 'Az Ön ügye ({{case_number}}) megoldódott', '<html><body><h1>Kedves {{customer_name}}!</h1><p>Az <b>{{case_number}}</b> számú ügye megoldásra került.</p><p><b>Megoldás:</b> {{resolution_text}}</p><p>Ha nem elégedett, kérjük 7 napon belül válaszoljon erre az e-mailre.</p></body></html>', 'yes', 'admin', '2023-04-01');

CREATE TABLE sla_rules (
    id VARCHAR(50),
    rule_name VARCHAR(255),
    case_type VARCHAR(100),
    priority VARCHAR(50),
    service_type VARCHAR(100),
    sla_hours VARCHAR(20),
    is_business_hours VARCHAR(10),
    escalation_after VARCHAR(50),
    escalation_target VARCHAR(255),
    is_active VARCHAR(10),
    created_date VARCHAR(50),
    notes VARCHAR(1000)
);

INSERT INTO sla_rules (id, rule_name, case_type, priority, service_type, sla_hours, is_business_hours, escalation_after, escalation_target, is_active, created_date, notes) VALUES
('SLA01', 'Standard lost parcel', 'LOST', 'any', 'standard', '48', 'yes', '36', 'Escalations department', 'yes', '2023-01-01', ''),
('SLA02', 'Express lost parcel', 'LOST', 'any', 'express', '12', 'yes', '8', 'Escalations department', 'yes', '2023-01-01', ''),
('SLA03', 'Damaged standard', 'DAMAGED', 'any', 'any', '48', 'yes', '36', 'Escalations department', 'yes', '2023-01-01', ''),
('SLA04', 'Damaged high priority', 'DAMAGED', 'high', 'any', '24', 'yes', '18', 'Maria Schmidt', 'yes', '2023-01-01', 'Overrides SLA03 for high priority'),
('SLA05', 'Delayed standard', 'DELAYED', 'any', 'standard', '48', 'no', '36', 'Escalations department', 'yes', '2023-01-01', 'Uses calendar hours not business hours'),
('SLA06', 'Delayed express', 'DELAYED', 'any', 'express', '12', 'yes', '8', 'Escalations department', 'yes', '2023-01-01', ''),
('SLA07', 'Billing any', 'BILLING', 'any', 'any', '72', 'yes', '48', 'Escalations department', 'yes', '2023-01-01', ''),
('SLA08', 'Wrong address', 'WRONG_ADDRESS', 'any', 'any', '48', 'yes', '36', 'Escalations department', 'yes', '2023-01-01', ''),
('SLA09', 'Other', 'OTHER', 'any', 'any', '72', 'yes', '48', 'Escalations department', 'yes', '2023-01-01', ''),
('SLA10', 'VIP override', 'any', 'high', 'any', '24', 'yes', '12', 'Maria Schmidt', 'yes', '2023-06-01', 'VIP customers always get 24h SLA. Which rule wins when both SLA04 and SLA10 match?'),
('SLA11', 'Critical everything', 'any', 'critical', 'any', '8', 'yes', '4', 'Elena Dvorak', 'yes', '2023-09-01', ''),
('SLA12', 'Same day rush', 'DELAYED', 'any', 'same_day', '4 hours', 'true', '2 hours', 'Maria Schmidt', 'yes', '2024-01-15', 'Added after same-day service launch. Note: hours appended to value'),
('SLA13', 'Standard lost parcel - updated', 'LOST', 'any', 'standard', '36', 'yes', '24', 'Escalations department', 'yes', '2024-02-01', 'Updated SLA but old rule SLA01 still active! Which one applies?'),
('SLA14', 'Express lost - disabled', 'LOST', 'high', 'express', '6', 'yes', '4', 'Elena Dvorak', 'no', '2024-01-01', 'Was too aggressive, disabled but kept for reference');

CREATE TABLE status_workflow (
    id VARCHAR(50),
    from_status VARCHAR(100),
    to_status VARCHAR(100),
    require_note VARCHAR(20),
    require_resolution VARCHAR(20),
    allowed_roles VARCHAR(500),
    is_active VARCHAR(10)
);

INSERT INTO status_workflow (id, from_status, to_status, require_note, require_resolution, allowed_roles, is_active) VALUES
('SW01', 'Open', 'In Progress', 'no', 'no', 'Customer Support,Escalations,Investigations', 'yes'),
('SW02', 'Open', 'Cancelled', 'yes', 'no', 'Customer Support,Escalations', 'yes'),
('SW03', 'In Progress', 'Awaiting Customer', 'yes', 'no', 'Customer Support,Escalations,Investigations', 'yes'),
('SW04', 'In Progress', 'Resolved', 'yes', 'yes', 'Customer Support,Escalations,Investigations', 'yes'),
('SW05', 'In Progress', 'Escalated', 'yes', 'no', 'Customer Support', 'yes'),
('SW06', 'Awaiting Customer', 'In Progress', 'no', 'no', 'Customer Support,Escalations,Investigations', 'yes'),
('SW07', 'Awaiting Customer', 'Resolved', 'yes', 'yes', 'Customer Support,Escalations,Investigations', 'yes'),
('SW08', 'Awaiting Customer', 'Cancelled', 'yes', 'no', 'Customer Support,Escalations', 'yes'),
('SW09', 'Resolved', 'Closed', 'no', 'no', 'Customer Support,Escalations,Investigations,System', 'yes'),
('SW10', 'Resolved', 'In Progress', 'yes', 'no', 'Customer Support,Escalations', 'yes'),
('SW11', 'Escalated', 'In Progress', 'yes', 'no', 'Escalations,Investigations', 'yes'),
('SW12', 'Escalated', 'Resolved', 'yes', 'yes', 'Escalations,Investigations', 'yes'),
('SW13', 'Closed', 'In Progress', 'yes', 'no', 'Escalations', 'yes'),
('SW14', 'Open', 'Resolved', 'no', 'no', 'Customer Support', 'yes'),
('SW15', 'Open', 'Closed', 'no', 'no', 'Customer Support', 'true'),
('SW16', 'Cancelled', 'Open', 'yes', 'no', 'Escalations', '1');

CREATE TABLE auto_assignment_rules (
    id VARCHAR(50),
    rule_name VARCHAR(255),
    priority VARCHAR(10),
    conditions VARCHAR(2000),
    assign_to_handler_id VARCHAR(50),
    assign_to_handler_name VARCHAR(255),
    assign_to_department VARCHAR(100),
    is_active VARCHAR(10),
    created_date VARCHAR(50),
    notes VARCHAR(1000)
);

INSERT INTO auto_assignment_rules (id, rule_name, priority, conditions, assign_to_handler_id, assign_to_handler_name, assign_to_department, is_active, created_date, notes) VALUES
('AR01', 'Budapest lost parcels', '1', '{"region":"Budapest","case_type":"LOST"}', '1', 'Anna Kovacs', '', 'yes', '2023-01-01', ''),
('AR02', 'Prague all cases', '3', 'region=Prague', '2', '', '', 'yes', '2023-01-01', 'Different condition format than AR01'),
('AR03', 'Vienna billing', '2', '{"region":"Vienna","case_type":"BILLING"}', '3', 'Maria Schmidt', '', 'yes', '2023-01-01', ''),
('AR04', 'Warsaw damaged', '1', '{"region":"Warsaw","case_type":"DAMAGED"}', '4', 'Jakub Wiśniewski', '', 'yes', '2023-01-01', ''),
('AR05', 'All escalations', '1', 'case_type=any AND priority=critical', '', '', 'Escalations', 'yes', '2023-06-01', 'Assigns to department not individual. Yet another condition format.'),
('AR06', 'Budapest general', '5', '{"region":"Budapest"}', '5', 'Peter Horvath', '', 'yes', '2023-01-01', 'Peter is inactive! Will break auto-assignment.'),
('AR07', 'Investigations fallback', '10', '{"is_escalated":"yes"}', '6', 'Elena Dvorak', '', 'yes', '2023-09-01', ''),
('AR08', 'Prague lost parcels', '1', '{"region":"Prague","case_type":"LOST"}', '2', 'Tomas Novak', '', 'yes', '2024-01-01', 'Same priority as AR01 for different region. But AR02 also matches Prague. Who wins?'),
('AR09', 'Vienna VIP', '0', '{"region":"Vienna","customer_vip":"yes"}', '3', '', 'Escalations', 'yes', '2024-01-15', 'Priority 0 = highest? Or is 1 highest? Not documented.'),
('AR10', 'Default fallback', '99', '{}', '', 'Unassigned', '', 'yes', '2023-01-01', 'Empty conditions = match everything? Handler "Unassigned" does not exist in handlers table.');

CREATE TABLE holidays (
    id VARCHAR(50),
    holiday_name VARCHAR(255),
    holiday_date VARCHAR(50),
    region VARCHAR(100),
    is_recurring VARCHAR(10),
    notes VARCHAR(500)
);

INSERT INTO holidays (id, holiday_name, holiday_date, region, is_recurring, notes) VALUES
('H01', 'New Year', '01-01', 'ALL', 'yes', ''),
('H02', 'Easter Monday', '2024-04-01', 'ALL', 'no', 'Date changes every year, needs manual update'),
('H03', 'March 15 - National Day', '03-15', 'HU', 'yes', ''),
('H04', 'St. Stephens Day', '08-20', 'HU', 'yes', ''),
('H05', 'October 23', '10-23', 'HU', 'yes', ''),
('H06', 'Austrian National Day', '10-26', 'AT', 'yes', ''),
('H07', 'May Day', '05-01', 'ALL', 'yes', ''),
('H08', 'Christmas', '12-25 - 12-26', 'ALL', 'yes', 'Two-day holiday encoded as range in single field'),
('H09', 'Czech Statehood Day', '09-28', 'CZ', 'yes', ''),
('H10', 'Polish Independence Day', '11-11', 'PL', 'yes', ''),
('H11', 'Whit Monday', '2024-05-20', 'AT,HU', 'no', 'Multiple regions in one field. Date varies yearly.'),
('H12', 'Good Friday', '29/03/2024', 'AT', 'no', 'Different date format. Also varies yearly.'),
('H13', 'Immaculate Conception', '12-08', 'AT', 'yes', ''),
('H14', 'Constitution Day (May 3)', '05-03', 'PL', 'yes', ''),
('H15', 'New Year (duplicate)', 'January 1', 'ALL', 'yes', 'Duplicate of H01 with different date format');

CREATE TABLE regions (
    id VARCHAR(50),
    region_name VARCHAR(100),
    country_code VARCHAR(10),
    country_name VARCHAR(100),
    timezone VARCHAR(100),
    business_hours_start VARCHAR(20),
    business_hours_end VARCHAR(20),
    business_days VARCHAR(50),
    manager_email VARCHAR(255),
    is_active VARCHAR(10)
);

INSERT INTO regions (id, region_name, country_code, country_name, timezone, business_hours_start, business_hours_end, business_days, manager_email, is_active) VALUES
('R01', 'Budapest', 'HU', 'Hungary', 'CET', '08:00', '18:00', 'Mon,Tue,Wed,Thu,Fri', 'bp.manager@swiftparcel.com', 'yes'),
('R02', 'Wien', 'AT', 'Austria', 'CET', '8:00', '17:00', '1,2,3,4,5', 'vienna.mgr@swiftparcel.com', 'yes'),
('R03', 'Prague', 'CZ', 'Czech Republic', 'CET', '09:00', '17:30', 'Monday-Friday', 'prague.manager@swiftparcel.com', 'yes'),
('R04', 'Warsaw', 'PL', 'Poland', 'CET', '8:00 AM', '5:00 PM', 'Mon-Fri', 'warsaw.mgr@swiftparcel.com', 'yes'),
('R05', 'Graz', 'AT', 'Austria', 'GMT+1', '08:00', '17:00', '1,2,3,4,5', 'graz.mgr@swiftparcel.com', 'yes'),
('R06', 'Linz', 'AT', 'Austria', 'UTC+1', '08:00', '17:00', 'Mon,Tue,Wed,Thu,Fri', '', 'yes'),
('R07', 'Bratislava', 'SK', 'Slovakia', 'CET', '08:00', '16:30', 'Mon-Fri', 'bratislava.mgr@swiftparcel.com', 'no');


CREATE TABLE roles (
    id VARCHAR(50),
    role_name VARCHAR(100),
    description VARCHAR(500),
    permissions VARCHAR(4000),
    can_access_all_regions VARCHAR(10),
    is_active VARCHAR(10),
    created_date VARCHAR(50)
);

INSERT INTO roles (id, role_name, description, permissions, can_access_all_regions, is_active, created_date) VALUES
('ROLE01', 'Read-Only', 'View only access for auditors and finance', 'case.view,note.view,parcel.view,customer.view,report.view', 'yes', 'yes', '2023-01-01'),
('ROLE02', 'Operator', 'Standard case handler', 'case.view,case.create,case.edit,case.assign,note.view,note.create,parcel.view,customer.view', 'no', 'yes', '2023-01-01'),
('ROLE03', 'Supervisor', 'Team supervisor with escalation rights', 'case.view,case.create,case.edit,case.assign,case.escalate,case.export,note.view,note.create,note.edit,parcel.view,customer.view,customer.edit,report.view,report.financial,handler.manage,sla.override', 'yes', 'yes', '2023-01-01'),
('ROLE04', 'Admin', 'Full system access', '*', 'yes', 'yes', '2023-01-01'),
('ROLE05', 'Senior Operator', 'Experienced operator with extra rights', 'case.view,case.create,case.edit,case.assign,case.escalate,note.view,note.create,parcel.view,customer.view,report.view', 'no', 'true', '2023-09-01'),
('ROLE06', 'Operator', 'Case handler - updated permissions', 'case.view,case.create,case.edit,case.assign,case.merge,note.view,note.create,parcel.view,customer.view', 'no', 'yes', '2024-01-15');

CREATE TABLE users (
    id VARCHAR(50),
    username VARCHAR(100),
    password VARCHAR(255),
    full_name VARCHAR(255),
    email VARCHAR(255),
    role VARCHAR(255),
    role_id VARCHAR(100),
    regions VARCHAR(500),
    is_active VARCHAR(10),
    last_login VARCHAR(50),
    created_date VARCHAR(50),
    created_by VARCHAR(100)
);

INSERT INTO users (id, username, password, full_name, email, role, role_id, regions, is_active, last_login, created_date, created_by) VALUES
('U01', 'anna.kovacs', 'Welcome123!', 'Anna Kovacs', 'anna.kovacs@swiftparcel.com', 'Operator', 'ROLE02', 'Budapest', 'yes', '2024-02-12 08:15', '2023-01-01', 'admin'),
('U02', 'tomas.novak', 'Parcel2023', 'Tomas Novak', 'tomas.novak@swiftparcel.com', 'Operator', 'ROLE02', 'Prague', 'yes', '2024-02-11 09:00', '2023-01-01', 'admin'),
('U03', 'maria.schmidt', 'M@ria2024!', 'Maria Schmidt', 'maria.schmidt@swiftparcel.com', 'Supervisor', 'ROLE03', 'Vienna,Graz,Linz', 'yes', '2024-02-12 07:45', '2023-01-01', 'admin'),
('U04', 'jakub.w', 'Krakow99', 'Jakub Wiśniewski', 'jakub.w@swiftparcel.com', 'Operator', 'ROLE02', 'Warsaw', 'yes', '2024-02-10 10:30', '2023-01-01', 'admin'),
('U05', 'peter.horvath', 'Budapest1', 'Peter Horvath', 'peter.horvath@swiftparcel.com', 'Operator', 'ROLE02', 'Budapest', 'yes', '2023-11-15 16:00', '2023-01-01', 'admin'),
('U06', 'elena.dvorak', 'Investigator1', 'Elena Dvorak', 'elena.dvorak@swiftparcel.com', 'Supervisor,Operator', 'ROLE03,ROLE02', 'Prague', 'yes', '2024-02-12 08:00', '2023-01-01', 'admin'),
('U07', 'admin', 'admin', 'System Administrator', 'admin@swiftparcel.com', 'Admin', 'ROLE04', '', 'yes', '2024-02-12 12:00', '2023-01-01', 'system'),
('U08', 'finance.audit', 'ReadOnly2023', 'Finance Auditor', 'finance@swiftparcel.com', 'Read-Only', 'ROLE01', '', 'yes', '2024-01-31 14:00', '2023-06-01', 'admin'),
('U09', 'manager.bp', 'Budapest2024', 'Budapest Manager', 'bp.manager@swiftparcel.com', '', '', 'Budapest', 'yes', '2024-02-01 09:00', '2024-01-01', 'admin'),
('U10', 'anna.kovacs2', 'Welcome456!', 'Anna Kovacs', 'anna.kovacs@swiftparcel.com', 'Senior Operator', 'ROLE05', 'Budapest', 'no', '2024-01-20 08:00', '2023-09-01', 'admin'),
('U11', 'ext.auditor', 'Audit2024!', 'External Auditor (PwC)', 'audit@pwc-example.com', 'Read-Only', 'ROLE01', 'Budapest,Vienna,Prague,Warsaw', 'yes', '2024-02-05 10:00', '2024-02-01', 'maria.schmidt');

CREATE TABLE user_permissions (
    id VARCHAR(50),
    user_id VARCHAR(50),
    username VARCHAR(100),
    permission VARCHAR(255),
    grant_type VARCHAR(20),
    granted_by VARCHAR(100),
    granted_date VARCHAR(50),
    expires VARCHAR(50),
    reason VARCHAR(500)
);

INSERT INTO user_permissions (id, user_id, username, permission, grant_type, granted_by, granted_date, expires, reason) VALUES
('UP01', 'U01', 'anna.kovacs', 'case.delete', 'grant', 'admin', '2024-01-15', '', 'Needed for cleanup of test cases during migration'),
('UP02', 'U01', 'anna.kovacs', 'case.export', 'grant', 'maria.schmidt', '2024-02-01', '2024-03-01', 'Temporary access for Q1 reporting'),
('UP03', 'U02', 'tomas.novak', 'report.view', 'grant', 'admin', '2024-01-01', NULL, 'Tomas preparing for supervisor promotion'),
('UP04', 'U04', 'jakub.w', 'customer.edit', 'grant', 'admin', '2023-06-01', '', ''),
('UP05', 'U06', 'elena.dvorak', 'config.sla.edit', 'grant', 'admin', '2023-09-01', NULL, 'Investigations team needs to adjust SLA for complex cases'),
('UP06', 'U01', 'anna.kovacs', 'case.escalate', 'deny', 'maria.schmidt', '2024-02-10', '', 'Escalation privilege revoked after incident on Feb 9'),
('UP07', 'U08', 'finance.audit', 'report.financial', 'grant', 'admin', '2023-06-01', '', 'Finance team needs access to refund reports'),
('UP08', '', 'manager.bp', 'handler.manage', 'grant', 'admin', '2024-01-01', NULL, 'No user_id filled in, matched by username only'),
('UP09', 'U03', 'maria.schmidt', 'case.delete', 'grant', 'admin', '2024-01-01', '2023-12-31', 'Already expired but still in table'),
('UP10', 'U05', 'peter.horvath', 'case.view', 'deny', 'admin', '2023-12-01', '', 'Access revoked when Peter left the team. But user is still active in users table!');

CREATE TABLE audit_log (
    id VARCHAR(50),
    action VARCHAR(255),
    entity_type VARCHAR(100),
    entity_id VARCHAR(100),
    user_name VARCHAR(255),
    user_id VARCHAR(50),
    old_value VARCHAR(4000),
    new_value VARCHAR(4000),
    timestamp VARCHAR(50),
    ip_address VARCHAR(50),
    details VARCHAR(4000)
);

INSERT INTO audit_log (id, action, entity_type, entity_id, user_name, user_id, old_value, new_value, timestamp, ip_address, details) VALUES
('AL01', 'CREATE', 'case', '1', 'tomas.novak', 'U02', '', '{"case_number":"CASE-2024-001","case_type":"LOST","status":"Open"}', '2024-01-20 10:00:00', '192.168.1.101', ''),
('AL02', 'STATUS_CHANGE', 'case', '1', 'tomas.novak', 'U02', 'Open', 'In Progress', '2024-01-20 10:05:00', '192.168.1.101', ''),
('AL03', 'case.create', 'case', '2', 'Jakub Wiśniewski', '', '', '', '2024-01-23 15:30:00', '10.0.0.55', 'Created damaged parcel case'),
('AL04', 'ASSIGN', 'case', '3', 'System', '', '', 'handler_id=1', '2024-01-14 09:00:00', '', 'Auto-assigned'),
('AL05', 'status_update', 'case', '3', 'anna.kovacs', 'U01', 'In Progress', 'Resolved', '2024-01-15 11:30:00', '192.168.1.100', ''),
('AL06', 'UPDATE', 'system_config', 'SC08', 'maria.schmidt', 'U03', '10', '15', '2024-02-01 09:00:00', '192.168.1.103', 'Updated max attachment size'),
('AL07', 'PERMISSION_GRANT', 'user_permission', 'UP01', 'admin', 'U07', '', 'case.delete', '2024-01-15 12:00:00', '192.168.1.1', ''),
('AL08', 'LOGIN', 'user', 'U07', 'admin', 'U07', '', '', '2024-02-12 12:00:00', '192.168.1.1', 'Successful login'),
('AL09', 'LOGIN_FAILED', 'user', '', 'admin', '', '', '', '2024-02-12 11:58:00', '45.33.22.11', 'Wrong password. 3rd attempt.'),
('AL10', 'login_fail', 'user', '', 'admin', '', '', '', '2024-02-12 11:57:00', '45.33.22.11', 'Same event, different action name than AL09'),
('AL11', 'ROLE_CHANGE', 'user', 'U10', 'admin', 'U07', 'Operator', 'Senior Operator', '2023-09-01 10:00:00', '192.168.1.1', ''),
('AL12', 'CREATE', 'case', '99', 'test.user', 'U99', '', '{"case_number":"CASE-TEST-001"}', '2024-01-10 00:00:00', '127.0.0.1', 'Test case - references non-existent user and case'),
('AL13', 'DELETE', 'case', '99', 'test.user', 'U99', '', '', '2024-01-10 00:01:00', '127.0.0.1', 'Deleted test case'),
('AL14', 'ESCALATE', 'case', '5', 'Maria Schmidt', 'U03', 'handler_id=3', 'handler_id=3', '2024-02-04 09:15:00', '192.168.1.103', 'Escalated to self? Old and new value are the same'),
('AL15', 'NOTE_ADD', 'case_note', 'N008', 'anna.kovacs', 'U01', '', '', '2024-02-11 10:00', '192.168.1.100', 'Missing seconds in timestamp'),
('AL16', 'config_update', 'system_config', 'SC14', 'tomas.novak', 'U02', 'yes', 'true', '10/02/2024 14:30:00', '192.168.1.101', 'Operator editing config? Should not have permission.');
