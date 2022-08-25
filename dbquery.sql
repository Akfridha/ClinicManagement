create database Hospital_Appointment_DB;
use Hospital_Appointment_DB; 


create table role(
 id int primary key identity(1,1), 
role_name varchar(20), 
cretaed_date datetime, 
updated_date datetime, 
isactive bit
)


create table User_master_table( 
 id int  primary key Identity(1,1), 
name varchar(50) not null, 
email varchar(50), 
phone varchar(20),
adress varchar(50), 
password varchar(10),
role_id int, 
created_date datetime,
updated_date datetime,
isactive bit,
FOREIGN KEY (role_id) REFERENCES role(id)
); 

create table appionment_booking(
id int primary key identity(1,1),
doctorId int not null, 
patientsID int not null,
start_dateTime datetime, 
end_dateTime datetime, 
booking_conformation bit,
created_date datetime, 
updated_date datetime,
isactive bit
)


insert into role values('Patient', GETDATE(), GETDATE(),1), ('Doctors', GETDATE(), GETDATE(),1), ('ClinicAdmins', GETDATE(), GETDATE(),1)

