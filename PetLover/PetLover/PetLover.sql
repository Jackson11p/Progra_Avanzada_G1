-- Crear la base de datos
CREATE DATABASE PetLover;
GO

-- Usar la base de datos creada
USE PetLover;
GO

--Tabla para almacenar los estados
CREATE TABLE Perfil(
	PerfilID INT PRIMARY KEY NOT NULL,
	Nombre varchar(50) NOT NULL
) 
GO
--Insertamos los perfiles
INSERT Perfil (PerfilID, Nombre) VALUES (1, N'Administrador')
GO
INSERT Perfil (PerfilID, Nombre) VALUES (2, N'Cliente')
GO

-- Tabla para almacenar información de los usuarios
CREATE TABLE Usuarios (
    UsuarioID INT PRIMARY KEY IDENTITY(1,1),
	Identificacion NVARCHAR(15) NOT NULL,
	Contrasenna varchar(15) NOT NULL, 
    Nombre NVARCHAR(100) NOT NULL,
	Correo NVARCHAR(100),
    Telefono NVARCHAR(15),
	Estado BIT NOT NULL,
	IdPerfil INT FOREIGN KEY REFERENCES Perfil(PerfilID)
);
GO
--Agregamos indices a datos unicos en la tabla Usarios
ALTER TABLE Usuarios
ADD CONSTRAINT UQ_Usuarios_Identificacion UNIQUE (Identificacion);
GO

ALTER TABLE Usuarios
ADD CONSTRAINT UQ_Usuarios_Email UNIQUE (Correo);
GO

ALTER TABLE Usuarios
ADD CONSTRAINT UQ_Usuarios_Telefono UNIQUE (Telefono);
GO

-- Tabla para almacenar información de las mascotas
CREATE TABLE Pets (
    MascotaID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Especie NVARCHAR(50),
    Raza NVARCHAR(50),
    FechaNacimiento DATE,
    ClienteID INT FOREIGN KEY REFERENCES Usuarios(UsuarioID)
);
GO

-- Tabla para almacenar información de los veterinarios
CREATE TABLE Veterinarios (
    VeterinarioID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(15),
    Email NVARCHAR(100),
    Especialidad NVARCHAR(100)
);
GO

-- Tabla para almacenar información de las citas
CREATE TABLE Citas (
    CitaID INT PRIMARY KEY IDENTITY(1,1),
    FechaHora DATETIME NOT NULL,
    MascotaID INT FOREIGN KEY REFERENCES Mascotas(MascotaID),
    VeterinarioID INT FOREIGN KEY REFERENCES Veterinarios(VeterinarioID),
    Descripcion NVARCHAR(MAX)
);
GO

-- Tabla para almacenar información de los tratamientos
CREATE TABLE Tratamientos (
    TratamientoID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(MAX),
    Costo DECIMAL(10, 2) NOT NULL
);
GO

-- Tabla para relacionar citas con tratamientos
CREATE TABLE CitaTratamientos (
    CitaID INT FOREIGN KEY REFERENCES Citas(CitaID),
    TratamientoID INT FOREIGN KEY REFERENCES Tratamientos(TratamientoID),
    PRIMARY KEY (CitaID, TratamientoID)
);
GO

-- Tabla para almacenar historial médico de las mascotas
CREATE TABLE HistorialMedico (
    HistorialID INT PRIMARY KEY IDENTITY(1,1),
    MascotaID INT FOREIGN KEY REFERENCES Mascotas(MascotaID),
    Fecha DATE NOT NULL,
    Diagnostico NVARCHAR(MAX),
    Tratamiento NVARCHAR(MAX),
    VeterinarioID INT FOREIGN KEY REFERENCES Veterinarios(VeterinarioID)
);
GO


--Procedimientos almacenados

--Registrar un usuario
CREATE PROCEDURE RegistrarCuenta
	@Identificacion varchar(15),
	@Contrasenna varchar(15),
	@Nombre varchar(200),
	@Correo varchar(200),
	@Telefono varchar(15)
AS
BEGIN
	
	INSERT INTO dbo.Usuarios(Identificacion,Contrasenna,Nombre,Correo,Telefono,Estado,IdPerfil)
	VALUES (@Identificacion,@Contrasenna,@Nombre,@Correo,@Telefono,1,2)

END
GO

CREATE OR ALTER PROCEDURE [dbo].[IniciarSesion]
	@Identificacion varchar(15),
	@Contrasenna varchar(15)
AS
BEGIN
	SELECT	U.UsuarioID, Identificacion, Contrasenna, U.Nombre 'NombreUsuario', Correo, Telefono, Estado, IdPerfil, P.Nombre 'NombrePerfil'
	FROM	dbo.Usuarios U
	INNER	JOIN dbo.Perfil P ON U.IdPerfil = P.PerfilID
	WHERE	Identificacion = @Identificacion
		AND Contrasenna = @Contrasenna
		AND Estado = 1
END
GO
