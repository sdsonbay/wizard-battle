using UnityEngine;
using System.Collections.Generic;

public class SpellPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject spellPrefab;
    
    public int initialPoolSize = 100;
    
    public int maxPoolSize = 250;
    
    public bool expandPool = true;

    [Header("Performance Metrics")]
    public int activeSpellCount = 0;
    
    public int poolSize = 0;

    private Queue<GameObject> spellPool = new Queue<GameObject>();
    private List<GameObject> activeSpells = new List<GameObject>();
    private Transform poolParent;

    void Awake()
    {
        GameObject poolObj = new GameObject("SpellPool_Container");
        poolObj.transform.SetParent(transform);
        poolParent = poolObj.transform;
        
        if (spellPrefab != null)
        {
            InitializePool();
        }
    }

    void Update()
    {
        activeSpellCount = activeSpells.Count;
        poolSize = spellPool.Count;
    }

    void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject spell = CreateNewSpell();
            spell.SetActive(false);
            spellPool.Enqueue(spell);
        }
    }

    GameObject CreateNewSpell()
    {
        GameObject spell = Instantiate(spellPrefab, poolParent);
        spell.name = "Spell_" + (spellPool.Count + activeSpells.Count);
        
        // Eski SpellProjectile component'ini kaldır (çakışmayı önlemek için)
        SpellProjectile oldScript = spell.GetComponent<SpellProjectile>();
        if (oldScript != null)
        {
            DestroyImmediate(oldScript);
        }
        
        PooledSpellProjectile spellScript = spell.GetComponent<PooledSpellProjectile>();
        if (spellScript == null)
        {
            spellScript = spell.AddComponent<PooledSpellProjectile>();
        }
        
        if (spellScript != null)
        {
            spellScript.SetPoolReference(this);
        }
        
        return spell;
    }

    public GameObject GetSpellFromPool(Vector3 position, Quaternion rotation, string ownerTag)
    {
        GameObject spell = null;

        if (spellPool.Count > 0)
        {
            spell = spellPool.Dequeue();
        }
        else if (expandPool && (maxPoolSize == 0 || (spellPool.Count + activeSpells.Count) < maxPoolSize))
        {
            spell = CreateNewSpell();
        }
        else if (activeSpells.Count > 0)
        {
            ReturnSpellToPool(activeSpells[0]);
            spell = spellPool.Dequeue();
        }

        if (spell != null)
        {
            // Önce parent'tan çıkar (pool container'dan)
            spell.transform.SetParent(null);
            
            // Pozisyon ve rotasyonu ayarla
            spell.transform.position = position;
            spell.transform.rotation = rotation;
            
            // Önce aktif et
            spell.SetActive(true);
            
            // Sonra Initialize çağır
            PooledSpellProjectile spellScript = spell.GetComponent<PooledSpellProjectile>();
            if (spellScript != null)
            {
                spellScript.Initialize(ownerTag);
            }
            else
            {
                Debug.LogError($"SpellPool: Spell'de PooledSpellProjectile component bulunamadı! Spell: {spell.name}. Spell prefab'ında PooledSpellProjectile component'i olduğundan emin olun!");
                spell.SetActive(false);
                spellPool.Enqueue(spell); // Geri ekle
                return null;
            }
            
            // Aktif listesine ekle
            if (!activeSpells.Contains(spell))
            {
                activeSpells.Add(spell);
            }
        }
        else
        {
            Debug.LogWarning("SpellPool: Pool'dan spell alınamadı! Pool boş ve genişletme kapalı veya max size'a ulaşıldı.");
        }

        return spell;
    }

    public void ReturnSpellToPool(GameObject spell)
    {
        if (spell == null) return;

        if (activeSpells.Contains(spell))
        {
            activeSpells.Remove(spell);
        }

        spell.SetActive(false);
        
        spell.transform.SetParent(poolParent);
        spell.transform.localPosition = Vector3.zero;
        spell.transform.localRotation = Quaternion.identity;
        spellPool.Enqueue(spell);
    }

    public void ReturnAllSpellsToPool()
    {
        while (activeSpells.Count > 0)
        {
            ReturnSpellToPool(activeSpells[0]);
        }
    }

    public void ClearPool()
    {
        ReturnAllSpellsToPool();
        
        while (spellPool.Count > 0)
        {
            GameObject spell = spellPool.Dequeue();
            if (spell != null)
            {
                Destroy(spell);
            }
        }
    }

    void OnDestroy()
    {
        ClearPool();
    }
}

