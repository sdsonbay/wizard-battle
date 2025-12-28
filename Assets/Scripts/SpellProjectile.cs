using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 5f;

    private float fixedY;
    private string ownerTag;

    void Start()
    {
        fixedY = transform.position.y;
        Destroy(gameObject, lifeTime);
    }

    public void SetOwnerTag(string tag)
    {
        ownerTag = tag;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos += transform.forward * speed * Time.deltaTime;
        pos.y = fixedY;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireWizard") || other.CompareTag("IceWizard"))
        {
            if (other.tag != ownerTag)
            {
                Destroy(gameObject);
            }
        }
    }
}