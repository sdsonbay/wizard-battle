using UnityEngine;

public class WizardSpawner : MonoBehaviour
{
    [Header("Wizard Prefabs")]
    public GameObject fireWizardPrefab;
    public GameObject iceWizardPrefab;

    [Header("Wizard Counts")]
    public int fireWizardCount = 5;
    public int iceWizardCount = 5;

    [Header("Spawn Areas")]
    public BoxCollider fireArea;
    public BoxCollider iceArea;

    void Start()
    {
        SpawnWizards(fireWizardPrefab, fireWizardCount, fireArea, "FireWizard");
        SpawnWizards(iceWizardPrefab, iceWizardCount, iceArea, "IceWizard");
    }

    void SpawnWizards(GameObject prefab, int count, BoxCollider area, string tagName)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetRandomPointInside(area);

            GameObject wizard = Instantiate(prefab, spawnPosition, Quaternion.identity);
            wizard.tag = tagName;
        }
    }

    Vector3 GetRandomPointInside(BoxCollider area)
    {
        Vector3 center = area.transform.position;
        Vector3 size = area.size;

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = center.y;
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);

        return new Vector3(x, y, z);
    }
}