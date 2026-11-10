public class FacilitiesLogic
{
    public List<FacilityModel> GetFacilities()
    {
        return FacilitiesAccess.GetAll();
    }

    public FacilityModel? GetFacility(int facilityId)
    {
        return FacilitiesAccess.GetById(facilityId);
    }

    public List<FacilityServiceModel> GetServices(int facilityId)
    {
        return FacilitiesAccess.GetServicesByFacilityId(facilityId);
    }

    public FacilityServiceModel? GetService(int serviceId)
    {
        return FacilitiesAccess.GetServiceById(serviceId);
    }
}
