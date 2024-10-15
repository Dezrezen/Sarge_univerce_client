using System.Collections.Generic;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.Interfaces
{
    public interface IBuildingsManager
    {
        void CreateBuilding(BuildingID buildingId);
        void ConfirmBuild();
        void CancelBuild();
        void MoveBuilding(long databaseId, int xPosition, int yPosition);
        void SyncBuildings(List<Data_Building> buildingsData);
        
    }
}