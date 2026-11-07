using Dapper;

public static class FacilitiesAccess
{
    public static List<FacilityModel> GetAll()
    {
        using var connection = DatabaseInitializer.Connect();

        return connection
            .Query<FacilityModel>(
                @"
            SELECT Facility_Id AS FacilityId, Name, Description,
                   Capacity, '0001-01-01 ' || OpeningHours AS OpeningHours,
                   '0001-01-01 ' || ClosingHours AS ClosingHours, Location
            FROM Facility
            ORDER BY Facility_Id;"
            )
            .ToList();
    }

    public static FacilityModel? GetById(int facilityId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.QueryFirstOrDefault<FacilityModel>(
            @"
            SELECT Facility_Id AS FacilityId, Name, Description,
                   Capacity, '0001-01-01 ' || OpeningHours AS OpeningHours,
                   '0001-01-01 ' || ClosingHours AS ClosingHours, Location
            FROM Facility
            WHERE Facility_Id = @FacilityId;",
            new { FacilityId = facilityId }
        );
    }

    public static List<FacilityServiceModel> GetServicesByFacilityId(int facilityId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection
            .Query<FacilityServiceModel>(
                @"
            SELECT Facility_Service_Id AS FacilityServiceId, Facility_Id AS FacilityId,
                   Name, Description, DurationMinutes
            FROM Facility_Service
            WHERE Facility_Id = @FacilityId
            ORDER BY DurationMinutes, Facility_Service_Id;",
                new { FacilityId = facilityId }
            )
            .ToList();
    }

    public static FacilityServiceModel? GetServiceById(int serviceId)
    {
        using var connection = DatabaseInitializer.Connect();

        return connection.QueryFirstOrDefault<FacilityServiceModel>(
            @"
            SELECT Facility_Service_Id AS FacilityServiceId, Facility_Id AS FacilityId,
                   Name, Description, DurationMinutes
            FROM Facility_Service
            WHERE Facility_Service_Id = @ServiceId;",
            new { ServiceId = serviceId }
        );
    }
}
