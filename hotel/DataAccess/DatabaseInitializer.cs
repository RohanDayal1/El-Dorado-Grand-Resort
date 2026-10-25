using Dapper;
using Microsoft.Data.Sqlite;

public static class DatabaseInitializer
{
    private const string ConnectionString = "Data Source=DataSources/el_dorado.db";
    public static SqliteConnection Connect() => new SqliteConnection(ConnectionString);

    public static void ResetDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        connection.Execute(@"
            DROP TABLE IF EXISTS Facility_Reservation;
            DROP TABLE IF EXISTS Activity_Reservation;
            DROP TABLE IF EXISTS Reservation_Room;
            DROP TABLE IF EXISTS Reservation;
            DROP TABLE IF EXISTS Room;
            DROP TABLE IF EXISTS Account;
            DROP TABLE IF EXISTS Facility;
            DROP TABLE IF EXISTS Activity;
            DROP TABLE IF EXISTS Room_Type;
            DROP TABLE IF EXISTS User;
        ");

        connection.Execute(@"
            CREATE TABLE User (
                User_Id INTEGER PRIMARY KEY,
                First_Name TEXT NOT NULL,
                Last_Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                Phone TEXT NOT NULL,
                Date_Of_Birth TEXT
            );

            CREATE TABLE Account (
                Account_Id INTEGER PRIMARY KEY,
                User_Id INTEGER NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Role TEXT NOT NULL DEFAULT 'guest' CHECK (Role IN ('guest','admin')),
                FOREIGN KEY (User_Id) REFERENCES User(User_Id) ON DELETE RESTRICT
            );

            CREATE TABLE Room_Type (
                Room_Type_Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT,
                Capacity INTEGER NOT NULL CHECK (Capacity > 0),
                PricePerNight REAL NOT NULL CHECK (PricePerNight >= 0),
                BedType TEXT
            );

            CREATE TABLE Room (
                Room_Id INTEGER PRIMARY KEY,
                Room_Type_Id INTEGER NOT NULL,
                RoomNumber INTEGER NOT NULL UNIQUE,
                FOREIGN KEY (Room_Type_Id) REFERENCES Room_Type(Room_Type_Id) ON DELETE RESTRICT
            );

            CREATE TABLE Reservation (
                Reservation_Id INTEGER PRIMARY KEY,
                ReservationCode TEXT NOT NULL UNIQUE,
                User_Id INTEGER NOT NULL,
                CheckInDate TEXT NOT NULL,
                CheckOutDate TEXT NOT NULL,
                Adults INTEGER NOT NULL CHECK (Adults >= 1),
                Children INTEGER NOT NULL CHECK (Children >= 0),
                TotalCost REAL NOT NULL DEFAULT 0,
                Status TEXT NOT NULL DEFAULT 'confirmed'
                    CHECK (Status IN ('confirmed','cancelled')),
                CHECK (CheckOutDate > CheckInDate),
                FOREIGN KEY (User_Id) REFERENCES User(User_Id) ON DELETE RESTRICT
            );

            CREATE TABLE Reservation_Room (
                Reservation_Id INTEGER NOT NULL,
                Room_Id INTEGER NOT NULL,
                PRIMARY KEY (Reservation_Id, Room_Id),
                FOREIGN KEY (Reservation_Id) REFERENCES Reservation(Reservation_Id) ON DELETE CASCADE,
                FOREIGN KEY (Room_Id) REFERENCES Room(Room_Id) ON DELETE RESTRICT
            );

            CREATE TABLE Facility (
                Facility_Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT,
                OpeningHours TEXT,
                ClosingHours TEXT,
                Location TEXT
            );

            CREATE TABLE Activity (
                Activity_Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT,
                Capacity INTEGER NOT NULL CHECK (Capacity > 0),
                OpeningHours TEXT,
                Location TEXT
            );

            CREATE TABLE Facility_Reservation (
                Facility_Reservation_Id INTEGER PRIMARY KEY,
                Account_Id INTEGER NOT NULL,
                Facility_Id INTEGER NOT NULL,
                ReservationDate TEXT NOT NULL,
                StartTime TEXT NOT NULL,
                EndTime TEXT NOT NULL,
                PartySize INTEGER NOT NULL CHECK (PartySize > 0),
                Status TEXT NOT NULL DEFAULT 'confirmed'
                    CHECK (Status IN ('confirmed','cancelled')),
                FOREIGN KEY (Account_Id) REFERENCES Account(Account_Id) ON DELETE RESTRICT,
                FOREIGN KEY (Facility_Id) REFERENCES Facility(Facility_Id) ON DELETE RESTRICT
            );

            CREATE TABLE Activity_Reservation (
                Activity_Reservation_Id INTEGER PRIMARY KEY,
                Account_Id INTEGER NOT NULL,
                Activity_Id INTEGER NOT NULL,
                ReservationDate TEXT NOT NULL,
                StartTime TEXT NOT NULL,
                EndTime TEXT NOT NULL,
                PartySize INTEGER NOT NULL CHECK (PartySize > 0),
                Status TEXT NOT NULL DEFAULT 'confirmed'
                    CHECK (Status IN ('confirmed','cancelled')),
                FOREIGN KEY (Account_Id) REFERENCES Account(Account_Id) ON DELETE RESTRICT,
                FOREIGN KEY (Activity_Id) REFERENCES Activity(Activity_Id) ON DELETE RESTRICT
            );
        ");

        connection.Execute(@"
            INSERT INTO User (User_Id, First_Name, Last_Name, Email, Phone, Date_Of_Birth) VALUES
                (1, 'Admin', 'Admin', 'admin@test.nl', '0612345678', '2000-06-20'),
                (2, 'Hans', 'Aura', 'hans@test.nl', '0612345678', '2000-06-20'),
                (3, 'Rohan', 'Dayal', 'rohan@test.nl', '0698765432', '1995-03-15'),
                (4, 'Azad', 'de Vries', 'azad@test.nl', '0687654321', '1988-11-22'),
                (5, 'Zeming', 'Mo', 'zeming@test.nl', '0676543210', '2001-07-08'),
                (6, 'Yassir', 'Gantaoui', 'yassir@test.nl', '0676543210', '2001-07-08'),
                (7, 'Emma', 'Jansen', 'emma@test.nl', '0681234567', '1995-03-15');

            -- User 7 has a room reservation without an account.
            INSERT INTO Account (Account_Id, User_Id, Password, Role) VALUES
                (1, 1, 'admin123', 'admin'),
                (2, 2, 'test123', 'guest'),
                (3, 3, 'test123', 'guest'),
                (4, 4, 'test123', 'guest'),
                (5, 5, 'test123', 'guest'),
                (6, 6, 'test123', 'guest');

            INSERT INTO Room_Type (Room_Type_Id, Name, Description, Capacity, PricePerNight, BedType) VALUES
                (1, 'Standard', 'Comfortable room for two guests', 2, 100.00, 'Double'),
                (2, 'Deluxe', 'Spacious room with a balcony', 2, 150.00, 'King'),
                (3, 'Family', 'Room for a family of four', 4, 200.00, 'Double and two singles'),
                (4, 'Suite', 'Luxury suite with a separate living area', 4, 300.00, 'King and sofa bed');

            INSERT INTO Room (Room_Id, Room_Type_Id, RoomNumber) VALUES
                (1, 1, 101), (2, 1, 102), (3, 2, 201), (4, 2, 202),
                (5, 3, 301), (6, 3, 302), (7, 4, 401), (8, 4, 402);

            INSERT INTO Reservation (Reservation_Id, ReservationCode, User_Id, CheckInDate, CheckOutDate, Adults, Children, TotalCost, Status) VALUES
                (1, 'K7M2QX', 2, '2026-11-01', '2026-11-04', 2, 0, 300.00, 'confirmed'),
                (2, 'P4TW9B', 3, '2026-11-10', '2026-11-12', 2, 2, 400.00, 'confirmed'),
                (3, 'H8ZN3C', 4, '2026-12-01', '2026-12-03', 3, 0, 500.00, 'cancelled'),
                (4, 'R2VD6L', 7, '2026-11-15', '2026-11-18', 2, 0, 600.00, 'confirmed');

            INSERT INTO Reservation_Room (Reservation_Id, Room_Id) VALUES
                (1, 1), (2, 5), (3, 2), (3, 3), (4, 6);

            INSERT INTO Facility (Facility_Id, Name, Description, OpeningHours, ClosingHours, Location) VALUES
                (1, 'Swimming Pool', 'Indoor heated swimming pool', '07:00', '22:00', 'Ground floor'),
                (2, 'Spa', 'Relaxation and wellness treatments', '09:00', '20:00', 'First floor'),
                (3, 'Restaurant', 'Breakfast, lunch and dinner', '07:00', '23:00', 'Ground floor');

            INSERT INTO Activity (Activity_Id, Name, Description, Capacity, OpeningHours, Location) VALUES
                (1, 'Yoga', 'Guided morning yoga session', 20, '08:00', 'Garden'),
                (2, 'City Tour', 'Guided tour of the city', 15, '10:00', 'Main entrance');

            INSERT INTO Facility_Reservation
                (Facility_Reservation_Id, Account_Id, Facility_Id, ReservationDate, StartTime, EndTime, PartySize, Status) VALUES
                (1, 2, 2, '2026-11-02', '10:00', '11:00', 2, 'confirmed'),
                (2, 3, 3, '2026-11-10', '18:00', '19:30', 4, 'cancelled');

            INSERT INTO Activity_Reservation
                (Activity_Reservation_Id, Account_Id, Activity_Id, ReservationDate, StartTime, EndTime, PartySize, Status) VALUES
                (1, 2, 1, '2026-11-03', '08:00', '09:00', 2, 'confirmed'),
                (2, 3, 2, '2026-11-11', '10:00', '12:00', 4, 'confirmed');
        ");

    }
}
