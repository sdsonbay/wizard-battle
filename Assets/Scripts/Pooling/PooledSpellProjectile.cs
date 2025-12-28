using UnityEngine;

public class PooledSpellProjectile : MonoBehaviour
{
    [Header("Spell Settings")]
    public float speed = 15f;
    public float lifeTime = 5f;

    private float fixedY;
    private string ownerTag;
    private SpellPool poolController;
    private float spawnTime;
    private bool isInitialized = false;

    public void SetPoolReference(SpellPool controller)
    {
        poolController = controller;
    }

    public void Initialize(string tag)
    {
        ownerTag = tag;
        fixedY = transform.position.y;
        spawnTime = Time.time;
        isInitialized = true;
        
        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void OnEnable()
    {
        // OnEnable SetActive(true) ile çağrılır
        // Eğer Initialize henüz çağrılmadıysa, sadece fixedY'yi ayarla
        // Initialize daha sonra çağrılacak (ama bu durumda Initialize önce çağrılıyor)
        if (!isInitialized)
        {
            // Initialize çağrılmamışsa, sadece fixedY'yi ayarla
            if (transform.position.y != 0)
            {
                fixedY = transform.position.y;
            }
        }
    }

    void OnDisable()
    {
        CancelInvoke();
        
        isInitialized = false;
        ownerTag = "";
        fixedY = 0f;
    }

    void Update()
    {
        if (!isInitialized) return;

        Vector3 pos = transform.position;
        pos += transform.forward * speed * Time.deltaTime;
        pos.y = fixedY;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isInitialized) return;

        if (other.CompareTag("FireWizard") || other.CompareTag("IceWizard"))
        {
            if (other.tag != ownerTag)
            {
                ReturnToPool();
            }
        }
    }

    void ReturnToPool()
    {
        CancelInvoke();
        
        if (poolController != null)
        {
            poolController.ReturnSpellToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ForceReturnToPool()
    {
        ReturnToPool();
    }
}

