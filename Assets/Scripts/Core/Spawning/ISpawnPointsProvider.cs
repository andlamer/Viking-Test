using UnityEngine;

namespace VikingTest
{
    public interface ISpawnPointsProvider
    {
        Vector3[] GetStartingPositions(int numberOfPoints);
        Vector3 GetRandomPositionOnField();
    }
}