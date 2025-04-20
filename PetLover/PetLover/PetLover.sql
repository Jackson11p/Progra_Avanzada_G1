-- Crear la base de datos
CREATE DATABASE PetLover;
GO

-- Usar la base de datos creada
USE PetLover;
GO

select * from perfil
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
INSERT Perfil (PerfilID, Nombre) VALUES (3, N'Veterinario')
GO
--UPDATE USUARIOS
--SET IdPerfil = 1
--WHERE UsuarioID = 3
--UPDATE USUARIOS
--SET Estado = 1
--WHERE UsuarioID = 3

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
CREATE TABLE Mascotas (
    MascotaID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Especie NVARCHAR(50),
    Raza NVARCHAR(50),
    FechaNacimiento DATE NOT NULL,
    Estado BIT NOT NULL,
    IDUsuario INT FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    Imagen NVARCHAR(MAX) NULL
);
GO
SELECT * FROM Mascotas
-- Tabla para almacenar información de las citas
CREATE TABLE Citas (
    CitaID INT PRIMARY KEY IDENTITY(1,1),
    FechaHora DATETIME NOT NULL,
    MascotaID INT FOREIGN KEY REFERENCES Mascotas(MascotaID),
    VeterinarioID INT FOREIGN KEY REFERENCES Usuarios(UsuarioID),
    Descripcion NVARCHAR(MAX),
	Estado INT FOREIGN KEY REFERENCES EstadosCita(EstadoID),
);
GO
CREATE TABLE EstadosCita (
    EstadoID INT PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
);

-- Insertar Estados cita
INSERT INTO EstadosCita VALUES (1, 'Pendiente'), (2, 'Completada'), (3, 'Cancelada');


-- Tabla para almacenar información de los tratamientos
CREATE TABLE Tratamientos (
    TratamientoID INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(MAX),
    Costo DECIMAL(10, 2) NOT NULL,
	Estado BIT NOT NULL,
);
GO

SELECT * FROM Tratamientos

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

--Tabla para almacenar los errores
CREATE TABLE Error(
	Id bigint PRIMARY KEY IDENTITY(1,1) NOT NULL,
	IdUsuario BIGINT NOT NULL,
	Mensaje NVARCHAR(max) NOT NULL,
	FechaHora DATETIME NOT NULL,
	Origen varchar(500) NOT NULL
);
GO

select *  from Error;
go
--Procedimientos almacenados

--Registrar un usuario
CREATE OR ALTER PROCEDURE RegistrarCuenta
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

CREATE OR ALTER PROCEDURE IniciarSesion
	@Correo varchar(100),
	@Contrasenna varchar(15)
AS
BEGIN
	SELECT	U.UsuarioID, Identificacion, Contrasenna, U.Nombre 'NombreUsuario', Correo, Telefono, Estado, IdPerfil, P.Nombre 'NombrePerfil'
	FROM	dbo.Usuarios U
	INNER	JOIN dbo.Perfil P ON U.IdPerfil = P.PerfilID
	WHERE	Correo = @Correo
		AND Contrasenna = @Contrasenna
		AND Estado = 1
END
GO

CREATE OR ALTER PROCEDURE RegistrarError
	@IdUsuario BIGINT,
	@Mensaje   NVARCHAR(max),
	@Origen	   NVARCHAR(500)
AS
BEGIN
	
	INSERT INTO dbo.Error (IdUsuario,Mensaje,FechaHora,Origen)
    VALUES (@IdUsuario, @Mensaje, GETDATE(), @Origen)
END
GO

CREATE OR ALTER PROCEDURE ActualizarContrasenna
    @Correo NVARCHAR(100),
    @NuevaContrasenna NVARCHAR(255)
AS
BEGIN
    UPDATE Usuarios
    SET Contrasenna = @NuevaContrasenna
    WHERE Correo = @Correo AND Estado = 1;
END;
GO

CREATE OR ALTER PROCEDURE ConsultarUsuarios
AS
BEGIN
    SELECT 
        u.UsuarioID,u.Identificacion, u.Nombre, u.Correo, u.Telefono, u.Estado, p.Nombre AS NombrePerfil 
    FROM Usuarios u
    INNER JOIN Perfil p ON p.PerfilID = u.IdPerfil
	WHERE u.Estado = 1
END;
GO

CREATE OR ALTER PROCEDURE ActualizarUsuario
    @UsuarioID INT,
    @Identificacion NVARCHAR(15),
    @Nombre NVARCHAR(100),
    @Correo NVARCHAR(100),
    @Telefono NVARCHAR(15),
    @Estado BIT,
    @IdPerfil INT
AS
BEGIN
    UPDATE Usuarios
    SET Identificacion = @Identificacion,
        Nombre = @Nombre,
        Correo = @Correo,
        Telefono = @Telefono,
        Estado = @Estado,
        IdPerfil = @IdPerfil
    WHERE UsuarioID = @UsuarioID;
END;
GO

CREATE OR ALTER PROCEDURE RegistrarMascota
    @Nombre NVARCHAR(100),
    @Especie NVARCHAR(50),
    @Raza NVARCHAR(50),
	@FechaNacimiento DATE,
	@Estado BIT,
    @IDUsuario INT,
	@Imagen NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Mascotas (Nombre, FechaNacimiento, Especie, Raza, Estado ,IDUsuario, Imagen)
    VALUES (@Nombre, @FechaNacimiento, @Especie, @Raza,@Estado,@IDUsuario, @Imagen);
END;
GO

CREATE OR ALTER PROCEDURE ActualizarMascota
    @MascotaID INT,
    @Nombre NVARCHAR(100),
    @Especie NVARCHAR(50),
    @Raza NVARCHAR(50),
    @FechaNacimiento DATE,
	@Estado BIT,
    @IDUsuario INT,
	@Imagen NVARCHAR(MAX)
AS
BEGIN
    UPDATE Mascotas
    SET Nombre = @Nombre,
        Especie = @Especie,
        Raza = @Raza,
        FechaNacimiento = @FechaNacimiento,
		Estado = @Estado,
        IDUsuario = @IDUsuario,
		Imagen = @Imagen
    WHERE MascotaID = @MascotaID;
END;
GO


CREATE OR ALTER PROCEDURE ConsultarMascotas
AS
BEGIN
    SELECT 
        m.MascotaID, m.Nombre, m.Especie, m.Raza, m.FechaNacimiento, m.Estado, u.Nombre AS Propietario
    FROM Mascotas m
    INNER JOIN Usuarios u ON u.UsuarioID = m.IDUsuario
	WHERE m.Estado = 1
END;
GO

CREATE OR ALTER PROCEDURE ConsultarMascotasInactivas
AS
BEGIN
    SELECT 
        m.MascotaID, m.Nombre, m.Especie, m.Raza, m.FechaNacimiento, m.Estado, u.Nombre AS Propietario 
    FROM Mascotas m
    INNER JOIN Usuarios u ON u.UsuarioID = m.IDUsuario
	WHERE m.Estado = 0
END;
GO

CREATE OR ALTER PROCEDURE ConsultarMascotasPorUsuario
    @UsuarioID INT
AS
BEGIN
    SELECT 
        MascotaID,
        Nombre,
        Especie,
        Raza,
        FechaNacimiento,
		Imagen
    FROM Mascotas
    WHERE IDUsuario = @UsuarioID;
END;
GO



CREATE OR ALTER PROCEDURE ActualizarImagenMascota
    @MascotaID INT,
    @RutaImagen NVARCHAR(200)
AS
BEGIN
    UPDATE Mascotas
    SET Imagen = @RutaImagen
    WHERE MascotaID = @MascotaID;
END
GO


CREATE OR ALTER PROCEDURE CargarUsuarios
AS
BEGIN
    SELECT UsuarioID, Nombre
    FROM Usuarios;
END;
GO

CREATE OR ALTER PROCEDURE CargarPerfiles
AS
BEGIN
    SELECT PerfilID, Nombre
    FROM Perfil;
END;
GO

CREATE OR ALTER PROCEDURE RegistrarTratamiento
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX),
    @Costo DECIMAL(10,2),
	@Estado BIT
AS
BEGIN
    INSERT INTO Tratamientos (Nombre, Descripcion, Costo, Estado)
    VALUES (@Nombre, @Descripcion, @Costo, @Estado)
END;
GO

CREATE OR ALTER PROCEDURE ConsultarTratamientos
AS
BEGIN
    SELECT TratamientoID, Nombre, Descripcion, Costo, Estado
    FROM Tratamientos
	WHERE Estado = 1
END;
GO

CREATE OR ALTER PROCEDURE ConsultarTratamientosInactivos
AS
BEGIN
    SELECT TratamientoID, Nombre, Descripcion, Costo, Estado
    FROM Tratamientos
	WHERE Estado = 0
END;
GO

CREATE OR ALTER PROCEDURE ActualizarTratamiento
    @TratamientoID INT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX),
    @Costo DECIMAL(10,2),
	@Estado BIT
AS
BEGIN
    UPDATE Tratamientos
    SET 
        Nombre = @Nombre,
        Descripcion = @Descripcion,
        Costo = @Costo,
		Estado = @Estado
    WHERE TratamientoID = @TratamientoID;
END;
GO

CREATE OR ALTER PROCEDURE ConsultarCitasFuturas
AS
BEGIN
   SELECT 
        c.CitaID,
        c.FechaHora,
        m.Nombre AS Mascota,
        v.Nombre AS Veterinario,
        c.Descripcion,
        e.Nombre AS Estado
    FROM Citas c
    INNER JOIN Mascotas m ON m.MascotaID = c.MascotaID
    INNER JOIN Usuarios v ON v.UsuarioID = c.VeterinarioID
    INNER JOIN EstadosCita e ON e.EstadoID = c.Estado
    WHERE c.FechaHora >= CAST(GETDATE() AS DATE) 
      AND c.Estado = 1 
    ORDER BY c.FechaHora ASC
END;
GO

CREATE OR ALTER PROCEDURE ConsultarCitasPasadasOCanceladas
AS
BEGIN
    SELECT 
        c.CitaID,
        c.FechaHora,
        m.Nombre AS Mascota,
        v.Nombre AS Veterinario,
        c.Descripcion,
        e.Nombre AS Estado
    FROM Citas c
    INNER JOIN Mascotas m ON m.MascotaID = c.MascotaID
    INNER JOIN Usuarios v ON v.UsuarioID = c.VeterinarioID
    INNER JOIN EstadosCita e ON e.EstadoID = c.Estado
    WHERE c.FechaHora < CAST(GETDATE() AS DATE) 
       OR c.Estado != 1 
    ORDER BY c.FechaHora DESC
END;
GO


CREATE OR ALTER PROCEDURE RegistrarCita
    @FechaHora DATETIME,
    @MascotaID INT,
    @VeterinarioID INT,
    @Descripcion NVARCHAR(MAX),
    @Estado INT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM Citas
        WHERE FechaHora = @FechaHora 
          AND VeterinarioID = @VeterinarioID 
          AND Estado = 1
    )
    BEGIN
        RETURN -1 
    END

    IF EXISTS (
        SELECT 1 FROM Citas
        WHERE FechaHora = @FechaHora 
          AND MascotaID = @MascotaID 
          AND Estado = 1
    )
    BEGIN
        RETURN -2 
    END


    INSERT INTO Citas (FechaHora, MascotaID, VeterinarioID, Descripcion, Estado)
    VALUES (@FechaHora, @MascotaID, @VeterinarioID, @Descripcion, @Estado)

    RETURN SCOPE_IDENTITY()
END

CREATE OR ALTER PROCEDURE ActualizarCita
    @CitaID INT,
    @FechaHora DATETIME,
    @MascotaID INT,
    @VeterinarioID INT,
    @Descripcion NVARCHAR(MAX),
    @Estado INT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM Citas
        WHERE FechaHora = @FechaHora 
          AND VeterinarioID = @VeterinarioID
          AND Estado = 1
          AND CitaID != @CitaID
    )
    BEGIN
        RETURN -1 
    END

    IF EXISTS (
        SELECT 1 FROM Citas
        WHERE FechaHora = @FechaHora 
          AND MascotaID = @MascotaID
          AND Estado = 1
          AND CitaID != @CitaID
    )
    BEGIN
        RETURN -2 
    END

    UPDATE Citas
    SET FechaHora = @FechaHora,
        MascotaID = @MascotaID,
        VeterinarioID = @VeterinarioID,
        Descripcion = @Descripcion,
        Estado = @Estado
    WHERE CitaID = @CitaID

    RETURN 1 
END;
GO

delete from citas
CREATE OR ALTER PROCEDURE CargarVeterinarios
AS
BEGIN
    SELECT UsuarioID, Nombre
    FROM Usuarios
	WHERE IdPerfil = 3;
END;
GO

select * from Mascotas
CREATE OR ALTER PROCEDURE CargarMascotas
AS
BEGIN
    SELECT MascotaID, Nombre
    FROM Mascotas;
END;
GO

CREATE OR ALTER PROCEDURE CargarEstadosCita
AS
BEGIN
    SELECT EstadoID, Nombre
    FROM EstadosCita;
END;
GO


SELECT * FROM CITAS
delete from Citas
