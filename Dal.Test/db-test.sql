/* Crear base de datos */
DROP DATABASE IF EXISTS test;

/* Crear base de datos */
CREATE DATABASE test;

/* Usar base de datos */
USE test;

/* Crear tabla de usuarios */
CREATE TABLE user (
  iduser int NOT NULL AUTO_INCREMENT,
  login varchar(100) NOT NULL,
  name varchar(100) NOT NULL,
  password varchar(128) NOT NULL,
  active tinyint NOT NULL,
  PRIMARY KEY (iduser),
  UNIQUE KEY UK_user_login (login)
);

/* Poblar tabla de usuarios */
INSERT INTO user (login, name, password, active) VALUES ('leandrobaena@gmail.com', 'Leandro Baena Torres', SHA2('Prueba123', 512), 1);
INSERT INTO user (login, name, password, active) VALUES ('actualizame@gmail.com', 'Karol Ximena Baena', SHA2('Prueba123', 512), 1);
INSERT INTO user (login, name, password, active) VALUES ('borrame@gmail.com', 'David Santiago Baena', SHA2('Prueba123', 512), 1);
INSERT INTO user (login, name, password, active) VALUES ('inactivo@gmail.com', 'Luz Marina Torres', SHA2('Prueba123', 512), 0);

/* Crear tabla de roles */
CREATE TABLE role (
  idrole INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(45) NOT NULL,
  PRIMARY KEY (idrole),
  UNIQUE INDEX UK_role_name (name ASC)
);

/* Poblar tabla de roles */
INSERT INTO role (name ) VALUES ('Administradores');
INSERT INTO role (name ) VALUES ('Actualizame');
INSERT INTO role (name ) VALUES ('Borrame');
INSERT INTO role (name ) VALUES ('Para probar user_role y application_role');

/* Crear tabla de usuario por rol */
CREATE TABLE user_role (
  iduserrole int NOT NULL AUTO_INCREMENT,
  iduser int NOT NULL,
  idrole int NOT NULL,
  PRIMARY KEY (iduserrole),
  UNIQUE KEY UK_user_role (iduser,idrole),
  KEY FK_user_role_user_idx (iduser),
  KEY FK_user_role_role_idx (idrole),
  CONSTRAINT FK_user_role_role FOREIGN KEY (idrole) REFERENCES role (idrole) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT FK_user_role_user FOREIGN KEY (iduser) REFERENCES user (iduser) ON DELETE CASCADE ON UPDATE CASCADE
);

/* Crear vista de usuario por rol */
CREATE VIEW v_user_role AS
SELECT
	ur.iduserrole, u.iduser, u.login,
    u.name as user, u.active, r.idrole,
    r.name as role
FROM
	user_role ur
    INNER JOIN user u ON ur.iduser = u.iduser
    INNER JOIN role r ON ur.idrole = r.idrole;

/* Poblar tabla de usuario por rol */
INSERT INTO user_role (iduser, idrole) VALUES (1, 1);
INSERT INTO user_role (iduser, idrole) VALUES (1, 2);
INSERT INTO user_role (iduser, idrole) VALUES (2, 1);
INSERT INTO user_role (iduser, idrole) VALUES (2, 2);

/* Crear tabla de aplicaciones */
CREATE TABLE application (
  idapplication INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(45) NOT NULL,
  PRIMARY KEY (idapplication),
  UNIQUE INDEX UK_application_name (name ASC)
);

/* Poblar tabla de usuario por rol */
INSERT INTO application (name) VALUES ('Autenticacion');
INSERT INTO application (name ) VALUES ('Actualizame');
INSERT INTO application (name ) VALUES ('Borrame');

/* Crear tabla de aplicaciones por rol */
CREATE TABLE application_role (
  idapplicationrole INT NOT NULL AUTO_INCREMENT,
  idapplication INT NOT NULL,
  idrole INT NOT NULL,
  PRIMARY KEY (idapplicationrole),
  UNIQUE INDEX UK_application_role (idapplication ASC, idrole ASC),
  INDEX FK_application_role_role_idx (idrole ASC),
  INDEX FK_application_role_application_idx (idapplication ASC),
  CONSTRAINT FK_application_role_application FOREIGN KEY (idapplication) REFERENCES application (idapplication) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT FK_application_role_role FOREIGN KEY (idrole) REFERENCES role (idrole) ON DELETE CASCADE ON UPDATE CASCADE
);

/* Crear vista de aplicaciones por rol */
CREATE VIEW v_application_role AS
SELECT
	ar.idapplicationrole,
	a.idapplication,
	a.name AS application,
	r.idrole,
	r.name AS role
FROM
	application_role ar
	INNER JOIN application a ON ar.idapplication = a.idapplication
	INNER JOIN role r ON ar.idrole = r.idrole;

/* Poblar tabla de aplicaciones por rol */
INSERT INTO application_role (idapplication, idrole) VALUES (1, 1);
INSERT INTO application_role (idapplication, idrole) VALUES (1, 2);
INSERT INTO application_role (idapplication, idrole) VALUES (2, 1);
INSERT INTO application_role (idapplication, idrole) VALUES (2, 2);

/* Crear tabla de registros de base de datos */
CREATE TABLE log_db (
  idlog BIGINT NOT NULL AUTO_INCREMENT,
  date DATETIME NOT NULL,
  action CHAR(1) NOT NULL,
  idtable BIGINT NOT NULL,
  `table` VARCHAR(200) NOT NULL,
  `sql` TEXT NOT NULL,
  iduser INT NOT NULL,
  PRIMARY KEY (idlog),
  INDEX FK_log_user_idx (iduser ASC),
  CONSTRAINT FK_log_user FOREIGN KEY (iduser) REFERENCES user (iduser) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Poblar tabla de registros de base de datos */
INSERT INTO log_db (date, action, idtable, `table`, `sql`, iduser) VALUES (NOW(), 'I', 1, 'Tabla1', 'INSERT INTO Tabla1 (campo1) VALUES (''prueba'')', 1);

/* Crear tabla de registros de componentes */
CREATE TABLE log_component (
  idlog BIGINT NOT NULL AUTO_INCREMENT,
  date DATETIME NOT NULL,
  type CHAR(1) NOT NULL,
  controller VARCHAR(45) NOT NULL,
  method VARCHAR(45) NOT NULL,
  input TEXT NOT NULL,
  output TEXT NOT NULL,
  iduser INT NULL,
  PRIMARY KEY (idlog),
  INDEX FK_log_component_user_idx (iduser ASC),
  CONSTRAINT FK_log_component_user FOREIGN KEY (iduser) REFERENCES user (iduser) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Poblar tabla de registros de componentes */
INSERT INTO log_component (date, type, controller, method, input, output, iduser) VALUES (NOW(), 'E', 'Log', 'Test', 'Entrada de prueba', 'Salida de prueba', 1);
INSERT INTO log_component (date, type, controller, method, input, output, iduser) VALUES (NOW(), 'I', 'Log2', 'Test2', 'Entrada de prueba 2', 'Salida de prueba 2', 1);

/* Crear tabla de plantillas */
CREATE TABLE template (
  idtemplate int NOT NULL AUTO_INCREMENT,
  name varchar(50) NOT NULL,
  content text NOT NULL,
  PRIMARY KEY (idtemplate)
);

/* Poblar tabla de plantillas */
INSERT INTO template (name, content) VALUES ('Plantilla de prueba', '<h1>Esta es una prueba hecha por #{user}#</h1>');
INSERT INTO template (name, content) VALUES ('Plantilla a actualizar', '<h1>Esta es una prueba hecha para #{actualizar}#</h1>');
INSERT INTO template (name, content) VALUES ('Plantilla a eliminar', '<h1>Esta es una prueba hecha para #{eliminar}#</h1>');

/* Crear tabla de notificaciones */
CREATE TABLE notification (
  idnotification bigint NOT NULL AUTO_INCREMENT,
  date datetime NOT NULL,
  `to` varchar(200) NOT NULL,
  subject varchar(200) NOT NULL,
  content text NOT NULL,
  iduser int NOT NULL,
  PRIMARY KEY (idnotification),
  KEY FK_user_idx (iduser),
  CONSTRAINT FK_user FOREIGN KEY (iduser) REFERENCES user (iduser) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Poblar tabla de notificaciones */
INSERT INTO notification (date, `to`, subject, content, iduser) VALUES (NOW(), 'leandrobaena@gmail.com', 'Correo de prueba', '<h1>Esta es una prueba hecha por leandrobaena@gmail.com</h1>', 1);

/* Crear tabla de paises */
CREATE TABLE country (
  idcountry int NOT NULL AUTO_INCREMENT,
  code char(2) NOT NULL,
  name varchar(100) NOT NULL,
  PRIMARY KEY (idcountry),
  UNIQUE KEY UK_country_code (code)
);

/* Poblar tabla de países */
INSERT INTO country (code, name) VALUES ('CO', 'Colombia');
INSERT INTO country (code, name) VALUES ('US', 'Estados unidos');
INSERT INTO country (code, name) VALUES ('EN', 'Inglaterra');

/* Crear tabla de ciudades */
CREATE TABLE city (
  idcity INT NOT NULL AUTO_INCREMENT,
  idcountry INT NOT NULL,
  code CHAR(3) NOT NULL,
  name VARCHAR(100) NOT NULL,
  PRIMARY KEY (idcity),
  INDEX FK_city_country_idx (idcountry ASC),
  UNIQUE INDEX UK_city (idcountry ASC, code ASC),
  CONSTRAINT FK_city_country FOREIGN KEY (idcountry) REFERENCES country (idcountry) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Crear vista de ciudades */
CREATE VIEW v_city AS
SELECT
	c.idcity,
	co.idcountry,
	co.code AS country_code,
	co.name AS country_name,
	c.code,
	c.name
FROM
	city c
	INNER JOIN country co ON c.idcountry = co.idcountry;

/* Poblar tabla de ciudades */
INSERT INTO city (idcountry, code, name) VALUES (1, 'BOG', 'Bogota');
INSERT INTO city (idcountry, code, name) VALUES (1, 'MED', 'Medellin');
INSERT INTO city (idcountry, code, name) VALUES (1, 'CAL', 'Cali');

/* Crear tabla de tipos de identificación */
CREATE TABLE identification_type (
  ididentificationtype INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(45) NOT NULL,
  PRIMARY KEY (ididentificationtype)
);

/* Poblar tabla de tipos de identificación */
INSERT INTO identification_type (name) VALUES ('Cedula ciudadania');
INSERT INTO identification_type (name) VALUES ('Cedula extranjeria');
INSERT INTO identification_type (name) VALUES ('Pasaporte');

/* Crear tabla de tipos de ingreso */
CREATE TABLE income_type (
  idincometype int NOT NULL AUTO_INCREMENT,
  code char(2) NOT NULL,
  name varchar(45) NOT NULL,
  PRIMARY KEY (idincometype),
  UNIQUE KEY UK_income_type (code)
);

/* Poblar tabla de tipos de ingreso */
INSERT INTO income_type (code, name) VALUES ('CI', 'Cuota inicial');
INSERT INTO income_type (code, name) VALUES ('CR', 'Credito cartera');
INSERT INTO income_type (code, name) VALUES ('FC', 'Factura');

/* Crear tabla de oficinas */
CREATE TABLE office (
  idoffice int NOT NULL AUTO_INCREMENT,
  idcity int NOT NULL,
  name varchar(100) NOT NULL,
  address varchar(200) NOT NULL,
  phone varchar(45) NOT NULL,
  active bit(1) NOT NULL,
  PRIMARY KEY (idoffice),
  KEY FK_office_city_idx (idcity),
  CONSTRAINT FK_office_city FOREIGN KEY (idcity) REFERENCES city (idcity) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Crear vista de oficinas */
CREATE VIEW v_office AS
SELECT
	o.idoffice,
	o.name,
	o.address,
	o.phone,
	o.active,
	c.idcity,
	c.name as city,
	c.code as city_code,
	c.idcountry,
	c.country_name,
	c.country_code
FROM
	office o
	INNER JOIN v_city c ON o.idcity = c.idcity;

/* Poblar tabla de oficinas */
INSERT INTO office (idcity, name, address, phone, active) VALUES (1, 'Castellana', 'Cl 95', '3151234567', 1);
INSERT INTO office (idcity, name, address, phone, active) VALUES (1, 'Kennedy', 'Cl 56 sur', '3151234568', 1);
INSERT INTO office (idcity, name, address, phone, active) VALUES (1, 'Venecia', 'Puente', '3151234569', 0);

/* Crear tabla de planes */
CREATE TABLE plan (
  idplan INT NOT NULL AUTO_INCREMENT,
  value FLOAT NOT NULL,
  initial_fee FLOAT NOT NULL,
  installments_number INT NOT NULL,
  installment_value FLOAT NOT NULL,
  active TINYINT NOT NULL,
  description VARCHAR(500) NOT NULL,
  PRIMARY KEY (idplan)
);

/* Poblar tabla de oficinas */
INSERT INTO plan (value, initial_fee, installments_number, installment_value, active, description) VALUES (3779100, 444600, 12, 444600, 1, 'PLAN COFREM 12 MESES');
INSERT INTO plan (value, initial_fee, installments_number, installment_value, active, description) VALUES (3779100, 282600, 15, 233100, 1, 'PLAN COFREM 15 MESES');
INSERT INTO plan (value, initial_fee, installments_number, installment_value, active, description) VALUES (3779100, 235350, 15, 236250, 1, 'PLAN COFREM 15 MESES ESPECIAL');

/* Crear tabla de parámetros */
CREATE TABLE parameter (
  idparameter int NOT NULL AUTO_INCREMENT,
  name varchar(100) NOT NULL,
  value varchar(200) NOT NULL,
  PRIMARY KEY (idparameter),
  UNIQUE KEY UK_parameter_name (name)
);

/* Poblar tabla de parámetros */
INSERT INTO parameter (name, value) VALUES ('Parametro 1', 'Valor 1');
INSERT INTO parameter (name, value) VALUES ('Parametro 2', 'Valor 2');
INSERT INTO parameter (name, value) VALUES ('Parametro 3', 'Valor 3');

/* Crear tabla de ejecutivos de cuenta */
CREATE TABLE account_executive (
  idaccountexecutive int NOT NULL AUTO_INCREMENT,
  name varchar(100) NOT NULL,
  ididentificationtype int NOT NULL,
  identification varchar(45) NOT NULL,
  PRIMARY KEY (idaccountexecutive),
  UNIQUE KEY UK_prod_executive (ididentificationtype,identification),
  KEY FK_account_executive_identification_type_idx (ididentificationtype),
  CONSTRAINT FK_account_executive_identification_type FOREIGN KEY (ididentificationtype) REFERENCES identification_type (ididentificationtype) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Crear vista de ejecutivos de cuenta */
CREATE VIEW v_account_executive AS
SELECT
	ae.idaccountexecutive, ae.name, ae.identification,
	it.ididentificationtype, it.name AS identificationtype
FROM
	account_executive ae
	INNER JOIN identification_type it on ae.ididentificationtype = it.ididentificationtype;

/* Poblar tabla de ejecutivos de cuenta */
INSERT INTO account_executive (name, ididentificationtype, identification) VALUES ('Leandro Baena Torres', 1, '123456789');
INSERT INTO account_executive (name, ididentificationtype, identification) VALUES ('David Santiago Baena Barreto', 1, '987654321');
INSERT INTO account_executive (name, ididentificationtype, identification) VALUES ('Karol Ximena Baena Barreto', 1, '147852369');
INSERT INTO account_executive (name, ididentificationtype, identification) VALUES ('Luz Marina Torres', 1, '852963741');

/* Crear tabla de tipos de cuenta */
CREATE TABLE account_type (
  idaccounttype int NOT NULL AUTO_INCREMENT,
  name varchar(45) NOT NULL,
  PRIMARY KEY (idaccounttype)
);

/* Poblar tabla de tipos de cuenta */
INSERT INTO account_type (name) VALUES ('Caja');
INSERT INTO account_type (name) VALUES ('Bancos');
INSERT INTO account_type (name) VALUES ('Otra');

/* Crear tabla de números de cuenta */
CREATE TABLE account_number (
  idaccountnumber int NOT NULL AUTO_INCREMENT,
  idaccounttype int NOT NULL,
  idcity int NOT NULL,
  number varchar(15) NOT NULL,
  PRIMARY KEY (idaccountnumber),
  KEY FK_account_number_type_idx (idaccounttype),
  KEY FK_account_number_city_idx (idcity),
  CONSTRAINT FK_account_number_city FOREIGN KEY (idcity) REFERENCES city (idcity) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_account_number_type FOREIGN KEY (idaccounttype) REFERENCES account_type (idaccounttype) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Crear vista de números de cuenta */
CREATE VIEW v_account_number AS
SELECT
	an.idaccountnumber,
	an.number,
	at.idaccounttype,
    at.name AS accounttype,
	c.idcity,
    c.code AS city_code,
    c.name AS city_name,
    c.idcountry,
    c.country_code,
    c.country_name
FROM
	account_number an
    INNER JOIN account_type at ON an.idaccounttype = at.idaccounttype
    INNER JOIN v_city c ON an.idcity = c.idcity;

/* Poblar tabla de números de cuenta */
INSERT INTO account_number (idaccounttype, idcity, number) VALUES (1, 1, '123456789');
INSERT INTO account_number (idaccounttype, idcity, number) VALUES (1, 1, '987654321');
INSERT INTO account_number (idaccounttype, idcity, number) VALUES (1, 1, '147258369');

/* Crear tabla de titulares */
CREATE TABLE owner (
  idowner INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(100) NOT NULL,
  identification VARCHAR(20) NOT NULL,
  ididentificationtype INT NOT NULL,
  address_home VARCHAR(500) NOT NULL,
  address_office VARCHAR(500) NOT NULL,
  phone_home VARCHAR(45) NOT NULL,
  phone_office VARCHAR(45) NOT NULL,
  email VARCHAR(200) NOT NULL,
  PRIMARY KEY (idowner),
  UNIQUE KEY UK_owner (identification ASC, ididentificationtype ASC),
  KEY FK_owner_identification_type_idx (ididentificationtype ASC),
  CONSTRAINT FK_owner_identification_type FOREIGN KEY (ididentificationtype) REFERENCES identification_type (ididentificationtype) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Crear vista de titulares */
CREATE VIEW v_owner AS
SELECT
	o.idowner,
	o.name,
	it.ididentificationtype,
	it.name AS identificationtype,
	o.identification,
	o.address_home,
	o.address_office,
	o.phone_home,
	o.phone_office,
	o.email
FROM
	owner o
	INNER JOIN identification_type it ON o.ididentificationtype = it.ididentificationtype;

/* Poblar tabla de números de cuenta */
INSERT INTO owner (name, identification, ididentificationtype, address_home, address_office, phone_home, phone_office, email) VALUES ('Leandro Baena Torres', '123456789', 1, 'CL 1 # 2 - 3', 'CL 4 # 5 - 6', '3121234567', '3127654321', 'leandrobaena@gmail.com');
INSERT INTO owner (name, identification, ididentificationtype, address_home, address_office, phone_home, phone_office, email) VALUES ('David Santiago Baena Barreto', '987654321', 1, 'CL 7 # 8 - 9', 'CL 10 # 11 - 12', '3151234567', '3157654321', 'dsantiagobaena@gmail.com');
INSERT INTO owner (name, identification, ididentificationtype, address_home, address_office, phone_home, phone_office, email) VALUES ('Karol Ximena Baena Brreto', '147258369', 1, 'CL 13 # 14 - 15', 'CL 16 # 17 - 18', '3201234567', '3207654321', 'kximenabaena@gmail.com');

/* Crear tabla de beneficiarios */
CREATE TABLE beneficiary (
  idbeneficiary INT NOT NULL AUTO_INCREMENT,
  idowner INT NOT NULL,
  name VARCHAR(100) NOT NULL,
  ididentificationtype INT NOT NULL,
  identification VARCHAR(45) NOT NULL,
  relationship VARCHAR(45) NOT NULL,
  PRIMARY KEY (idbeneficiary),
  INDEX FK_beneficiary_owner_idx (idowner ASC),
  INDEX FK_beneficiary_identification_type_idx (ididentificationtype ASC),
  UNIQUE INDEX UK_beneficiary (idowner ASC, ididentificationtype ASC, identification ASC),
  CONSTRAINT FK_beneficiary_owner FOREIGN KEY (idowner) REFERENCES owner (idowner) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_beneficiary_identification_type FOREIGN KEY (ididentificationtype) REFERENCES identification_type (ididentificationtype) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Crear vista de beneficiarios */
CREATE VIEW v_beneficiary AS
SELECT
	b.idbeneficiary,
	b.idowner,
	o.name AS owner,
	o.ididentificationtype AS owner_ididentificationtype,
	o.identificationtype AS owner_identificationtype,
	o.identification AS owner_identification,
	o.address_home AS owner_address_home,
	o.address_office AS owner_address_office,
	o.phone_home AS owner_phone_home,
	o.phone_office AS owner_phone_office,
	o.email AS owner_email,
	b.name,
	b.ididentificationtype,
	it.name AS identificationtype,
	b.identification,
	b.relationship
FROM
	beneficiary b
	INNER JOIN identification_type it ON b.ididentificationtype = it.ididentificationtype
	INNER JOIN v_owner o ON b.idowner = o.idowner;

/* Poblar tabla de beneficiarios */
INSERT INTO beneficiary (idowner, name, ididentificationtype, identification, relationship) VALUES (1, 'Pedro Perez', 1, '111111111', 'hijo');
INSERT INTO beneficiary (idowner, name, ididentificationtype, identification, relationship) VALUES (1, 'Maria Martinez', 1, '222222222', 'hija');
INSERT INTO beneficiary (idowner, name, ididentificationtype, identification, relationship) VALUES (1, 'Hernan Hernandez', 1, '333333333', 'esposa');
INSERT INTO beneficiary (idowner, name, ididentificationtype, identification, relationship) VALUES (1, 'Para eliminar', 1, '1111122222', 'primo');

/* Crear tabla de tipos de consecutivo */
CREATE TABLE consecutive_type (
  idconsecutivetype int NOT NULL AUTO_INCREMENT,
  name varchar(45) NOT NULL,
  PRIMARY KEY (idconsecutivetype)
);

/* Poblar tabla de tipos de consecutivo */
INSERT INTO consecutive_type (name) VALUES ('Recibos de caja');
INSERT INTO consecutive_type (name) VALUES ('Registro oficial');
INSERT INTO consecutive_type (name) VALUES ('Otro registro');

/* Crear tabla de números de consecutivo */
CREATE TABLE consecutive_number (
  idconsecutivenumber int NOT NULL AUTO_INCREMENT,
  idconsecutivetype int NOT NULL,
  idcity int NOT NULL,
  number varchar(45) NOT NULL,
  PRIMARY KEY (idconsecutivenumber),
  KEY FK_consecutive_number_type_idx (idconsecutivetype),
  KEY FK_consecutive_number_city_idx (idcity),
  CONSTRAINT FK_consecutive_number_city FOREIGN KEY (idcity) REFERENCES city (idcity) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_consecutive_number_type FOREIGN KEY (idconsecutivetype) REFERENCES consecutive_type (idconsecutivetype) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Crear vista de números de consecutivo */
CREATE VIEW v_consecutive_number AS
SELECT
	cn.idconsecutivenumber,
	cn.number,
	ct.idconsecutivetype,
    ct.name AS consecutivetype,
	c.idcity,
    c.code AS city_code,
    c.name AS city_name,
    c.idcountry,
    c.country_code,
    c.country_name
FROM
	consecutive_number cn
    INNER JOIN consecutive_type ct ON cn.idconsecutivetype = ct.idconsecutivetype
    INNER JOIN v_city c ON cn.idcity = c.idcity;

/* Poblar tabla de números de consecutivo */
INSERT INTO consecutive_number (idconsecutivetype, idcity, number) VALUES (1, 1, '9999999999');
INSERT INTO consecutive_number (idconsecutivetype, idcity, number) VALUES (1, 1, '8888888888');
INSERT INTO consecutive_number (idconsecutivetype, idcity, number) VALUES (1, 1, '7777777777');

/* Crear tabla de ejecutivos de cuenta en oficinas */
CREATE TABLE executive_office (
  idexecutiveoffice int NOT NULL AUTO_INCREMENT,
  idoffice int NOT NULL,
  idaccountexecutive int NOT NULL,
  PRIMARY KEY (idexecutiveoffice),
  UNIQUE KEY UK_executive_office (idoffice,idaccountexecutive),
  KEY FK_executive_office_office_idx (idoffice),
  KEY FK_executive_office_executive_idx (idaccountexecutive),
  CONSTRAINT FK_executive_office_executive FOREIGN KEY (idaccountexecutive) REFERENCES account_executive (idaccountexecutive) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_executive_office_office FOREIGN KEY (idoffice) REFERENCES office (idoffice)
);

/* Crear vista de ejecutivos de cuenta en oficinas */
CREATE VIEW v_executive_office AS
SELECT
	eo.idexecutiveoffice,
    eo.idoffice,
    ae.idaccountexecutive,
    ae.name as accountexecutive,
    ae.identification,
    ae.ididentificationtype,
    ae.identificationtype
FROM 
	executive_office eo
    INNER JOIN v_account_executive ae ON eo.idaccountexecutive = ae.idaccountexecutive;

/* Poblar tabla de ejecutivos de cuenta en oficinas */
INSERT INTO executive_office (idoffice, idaccountexecutive) VALUES (1, 1);
INSERT INTO executive_office (idoffice, idaccountexecutive) VALUES (1, 2);
INSERT INTO executive_office (idoffice, idaccountexecutive) VALUES (2, 1);

/* Crear tabla de escalas */
CREATE TABLE scale (
  idscale int NOT NULL AUTO_INCREMENT,
  code varchar(5) NOT NULL,
  name varchar(45) NOT NULL,
  comission int NOT NULL,
  `order` smallint NOT NULL,
  PRIMARY KEY (idscale)
);

/* Poblar tabla de escalas */
INSERT INTO scale (code, name, comission, `order`) VALUES ('C1', 'Comision 1', 1000, 1);
INSERT INTO scale (code, name, comission, `order`) VALUES ('C2', 'Comision 2', 2000, 2);
INSERT INTO scale (code, name, comission, `order`) VALUES ('C3', 'Comision 3', 3000, 3);

/* Crear tabla de matriculas */
CREATE TABLE registration (
  idregistration int NOT NULL AUTO_INCREMENT,
  idoffice int NOT NULL,
  date date NOT NULL,
  contract_number varchar(10) NOT NULL,
  idowner int NOT NULL,
  idbeneficiary1 int DEFAULT NULL,
  idbeneficiary2 int DEFAULT NULL,
  idplan int NOT NULL,
  PRIMARY KEY (idregistration),
  KEY FK_registration_office_idx (idoffice),
  KEY FK_registration_owner_idx (idowner),
  KEY FK_registration_beneficiary1_idx (idbeneficiary1),
  KEY FK_registration_beneficiary2_idx (idbeneficiary2),
  KEY FK_registration_plan_idx (idplan),
  CONSTRAINT FK_registration_beneficiary1 FOREIGN KEY (idbeneficiary1) REFERENCES beneficiary (idbeneficiary) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_registration_beneficiary2 FOREIGN KEY (idbeneficiary2) REFERENCES beneficiary (idbeneficiary) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_registration_office FOREIGN KEY (idoffice) REFERENCES office (idoffice) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_registration_owner FOREIGN KEY (idowner) REFERENCES owner (idowner) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_registration_plan FOREIGN KEY (idplan) REFERENCES plan (idplan) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Crear tabla de matriculas */
CREATE VIEW v_registration AS
SELECT
	r.idregistration, r.date, r.contract_number,
    o.idoffice, o.name as office, o.address as office_address,
	o.phone as office_phone, o.active as office_active, o.idcity as office_idcity,
	o.city_code as office_city_code, o.city as office_city_name, o.idcountry as office_idcountry, o.country_code as office_country_code,
	o.country_name as office_country_name, ow.idowner, ow.name as owner,
	ow.ididentificationtype as owner_ididentificationtype, ow.identificationtype as owner_identificationtype, ow.identification as owner_identification,
	ow.address_home as owner_address_home, ow.address_office as owner_address_office, ow.phone_home as owner_phone_home,
	ow.phone_office as owner_phone_office, ow.email as owner_email, b1.idbeneficiary as idbeneficiary1,
	b1.name as beneficiary1, b1.ididentificationtype as beneficiary1_ididentificationtype, b1.identificationtype as beneficiary1_identificationtype,
	b1.identification as beneficiary1_identification, b1.relationship as beneficiary1_relationship, b2.idbeneficiary as idbeneficiary2,
	b2.name as beneficiary2, b2.ididentificationtype as beneficiary2_ididentificationtype, b2.identificationtype as beneficiary2_identificationtype,
	b2.identification as beneficiary2_identification, b2.relationship as beneficiary2_relationship, p.idplan,
	p.value as plan_value, p.initial_fee as plan_initial_fee, p.installments_number as plan_installments_number,
	p.installment_value as plan_installment_value, p.active as plan_active, p.description as plan_description
FROM
	registration r
    INNER JOIN v_office o ON r.idoffice = o.idoffice
    INNER JOIN v_owner ow ON r.idowner = ow.idowner
	INNER JOIN plan p ON r.idplan = p.idplan
    LEFT JOIN v_beneficiary b1 ON r.idbeneficiary1 = b1.idbeneficiary
	LEFT JOIN v_beneficiary b2 ON r.idbeneficiary2 = b2.idbeneficiary;

/* Poblar tabla de matriculas */
INSERT INTO registration (idoffice, date, contract_number, idowner, idbeneficiary1, idbeneficiary2, idplan) VALUES (1, CURDATE(), '255657', 1, 1, 2, 1);
INSERT INTO registration (idoffice, date, contract_number, idowner, idbeneficiary1, idbeneficiary2, idplan) VALUES (1, CURDATE(), '256566', 1, 3, NULL, 1);
INSERT INTO registration (idoffice, date, contract_number, idowner, idbeneficiary1, idbeneficiary2, idplan) VALUES (1, CURDATE(), '255658', 1, NULL, NULL, 2);

/* Crear tabla de escalas en matriculas */
CREATE TABLE registration_scale (
  idregistrationscale int NOT NULL AUTO_INCREMENT,
  idregistration int NOT NULL,
  idscale int NOT NULL,
  idaccountexecutive int NOT NULL,
  PRIMARY KEY (idregistrationscale),
  KEY FK_registration_scale_registration_idx (idregistration),
  KEY FK_registration_scale_scale_idx (idscale),
  KEY FK_registration_scale_executive_idx (idaccountexecutive),
  CONSTRAINT FK_registration_scale_executive FOREIGN KEY (idaccountexecutive) REFERENCES account_executive (idaccountexecutive) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_registration_scale_registration FOREIGN KEY (idregistration) REFERENCES registration (idregistration) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_registration_scale_scale FOREIGN KEY (idscale) REFERENCES scale (idscale) ON DELETE RESTRICT ON UPDATE RESTRICT
);

/* Crear vista de escalas en matriculas */
CREATE VIEW v_registration_scale AS
SELECT
	rs.idregistrationscale,
    rs.idregistration,
    s.idscale,
    s.code as scale_code,
    s.name as scale,
    s.comission as scale_comission,
    s.`order` as scale_order,
    ae.idaccountexecutive,
    ae.name as accountexecutive,
    ae.identification as account_executive_identification,
    ae.ididentificationtype as account_executive_ididentificationtype,
    ae.identificationtype as account_executive_identificationtype
FROM
	registration_scale rs
    INNER JOIN scale s ON rs.idscale = s.idscale
    INNER JOIN v_account_executive ae ON rs.idaccountexecutive = ae.idaccountexecutive;

/* Poblar tabla de escalas en matriculas */
INSERT INTO registration_scale (idregistration, idscale, idaccountexecutive) VALUES (1, 1, 1);
INSERT INTO registration_scale (idregistration, idscale, idaccountexecutive) VALUES (1, 1, 2);
INSERT INTO registration_scale (idregistration, idscale, idaccountexecutive) VALUES (1, 2, 1);

/* Crear tabla de cuotas de una matrícula */
CREATE TABLE fee (
  idfee INT NOT NULL AUTO_INCREMENT,
  idregistration INT NOT NULL,
  value FLOAT NOT NULL,
  number SMALLINT NOT NULL,
  idincometype INT NOT NULL,
  dueDate DATE NOT NULL,
  PRIMARY KEY (idfee),
  UNIQUE KEY UK_fee (idregistration, number, idincometype),
  INDEX FK_fee_registration_idx (idregistration ASC),
  INDEX FK_fee_income_type_idx (idincometype ASC),
  CONSTRAINT FK_fee_registration FOREIGN KEY (idregistration) REFERENCES registration (idregistration) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT FK_fee_income_type FOREIGN KEY (idincometype) REFERENCES income_type (idincometype) ON DELETE NO ACTION ON UPDATE NO ACTION
);

/* Crear vista de cuotas de una matrícula */
CREATE VIEW v_fee AS
SELECT
 f.idfee,
 f.idregistration,
 f.value,
 f.number,
 f.dueDate,
 it.idincometype,
 it.code AS incometype_code,
 it.name AS incometype
FROM
 fee f
 INNER JOIN income_type it ON f.idincometype = it.idincometype;

/* Poblar tabla de cuotas de una matricula */
INSERT INTO fee (idregistration, value, number, idincometype, dueDate) VALUES (1, 1000, 1, 1, CURDATE());
INSERT INTO fee (idregistration, value, number, idincometype, dueDate) VALUES (1, 2000, 2, 1, CURDATE());
INSERT INTO fee (idregistration, value, number, idincometype, dueDate) VALUES (1, 3000, 3, 1, CURDATE());

/* Crear tabla de tipos de pago */
CREATE TABLE payment_type (
  idpaymenttype INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(45) NOT NULL,
  PRIMARY KEY (idpaymenttype)
);

/* Poblar tabla de tipos de pago */
INSERT INTO payment_type (name) VALUES ('Efectivo');
INSERT INTO payment_type (name) VALUES ('Nequi');
INSERT INTO payment_type (name) VALUES ('PSE');

/* Crear tabla de pagos */
CREATE TABLE payment (
  idpayment BIGINT NOT NULL AUTO_INCREMENT,
  idpaymenttype INT NOT NULL,
  idfee INT NOT NULL,
  value FLOAT NOT NULL,
  date DATE NOT NULL,
  invoice VARCHAR(20) NOT NULL,
  proof VARCHAR(300) NOT NULL,
  PRIMARY KEY (idpayment),
  INDEX FK_payment_type_idx (idpaymenttype ASC),
  INDEX FK_payment_fee_idx (idfee ASC),
  CONSTRAINT FK_payment_type FOREIGN KEY (idpaymenttype) REFERENCES payment_type (idpaymenttype) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT FK_payment_fee FOREIGN KEY (idfee) REFERENCES fee (idfee) ON DELETE NO ACTION ON UPDATE NO ACTION
);

/* Crear vista de pagos */
CREATE VIEW v_payment AS
SELECT
	p.idpayment,
    pt.idpaymenttype,
    pt.name AS paymenttype,
    f.idfee,
    p.value,
    p.date,
    p.invoice,
    p.proof
FROM
	payment p
    INNER JOIN payment_type pt ON p.idpaymenttype = pt.idpaymenttype
    INNER JOIN fee f ON p.idfee = f.idfee;

/* Poblar tabla de pago */
INSERT INTO payment (idpaymenttype, idfee, value, date, invoice, proof) VALUES (1, 1, 1500, CURDATE(), '101-000001', 'http://localhost/prueba1.png');
INSERT INTO payment (idpaymenttype, idfee, value, date, invoice, proof) VALUES (1, 1, 2500, CURDATE(), '101-000002', 'http://localhost/prueba2.png');
INSERT INTO payment (idpaymenttype, idfee, value, date, invoice, proof) VALUES (1, 1, 3500, CURDATE(), '101-000003', 'http://localhost/prueba3.png');
