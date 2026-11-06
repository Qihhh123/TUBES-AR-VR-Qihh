using UnityEngine;
using UnityEngine.XR.ARFoundation;

[System.Serializable]
public class FilterAdjust
{
    public Vector3 positionOffset;       // geser (satuan meter, 0.01 = 1 cm)
    public Vector3 rotationOffset;       // putar (derajat)
    public float scaleMultiplier = 1f;   // pengali ukuran (0.8 = kecilkan 20%)
}

public class FaceFilterSwitcher : MonoBehaviour
{
    public ARFaceManager faceManager;
    public GameObject[] facePrefabs;      // Kacamata, Mask, Jenggot, Headphone
    public FilterAdjust[] adjustments;    // urutannya sama dengan facePrefabs

    private int currentIndex = 0;

    void OnEnable()
    {
        faceManager.trackablesChanged.AddListener(OnFacesChanged);
    }

    void OnDisable()
    {
        faceManager.trackablesChanged.RemoveListener(OnFacesChanged);
    }

    void OnFacesChanged(ARTrackablesChangedEventArgs<ARFace> args)
    {
        foreach (var face in args.added)
            ApplyFilter(face);
    }

    public void SwitchModel(int index)
    {
        if (index < 0 || index >= facePrefabs.Length) return;
        currentIndex = index;

        foreach (var face in faceManager.trackables)
            ApplyFilter(face);
    }

    void ApplyFilter(ARFace face)
    {
        foreach (Transform child in face.transform)
            Destroy(child.gameObject);

        GameObject obj = Instantiate(facePrefabs[currentIndex], face.transform);

        if (adjustments != null && currentIndex < adjustments.Length)
        {
            FilterAdjust adj = adjustments[currentIndex];
            float s = adj.scaleMultiplier > 0f ? adj.scaleMultiplier : 1f;

            obj.transform.localPosition += adj.positionOffset;
            obj.transform.localRotation = Quaternion.Euler(adj.rotationOffset) * obj.transform.localRotation;
            obj.transform.localScale *= s;
        }
    }
}