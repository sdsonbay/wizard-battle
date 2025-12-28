using UnityEngine;

public class WizardAI : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stopDistance = 8f;
    public float attackRange = 25f;
    public float minAttackInterval = 3f;
    public float maxAttackInterval = 6f;
    private float attackInterval = 3f;

    public Animator animator;
    public GameObject spellPrefab;
    public Transform spellSpawnPoint;

    public Transform target;
    public float attackTimer = 0f;
    public bool isAttacking = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        
        AssignTarget();
        transform.position = new Vector3(transform.position.x, -0.977F, transform.position.z);
    }

    void Update()
    {
        if (target == null)
        {
            AssignTarget();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance && !isAttacking)
            MoveTowardsTarget();
        else if (!isAttacking)
            StopMoving();

        if (isAttacking && animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.95f)
            {
                EndAttack();
            }
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f && distance <= attackRange && !isAttacking)
        {
            // Rastgele attack interval belirle
            attackInterval = Random.Range(minAttackInterval, maxAttackInterval);
            attackTimer = attackInterval;
            StartAttack();
        }
    }

    void AssignTarget()
    {
        string enemyTag = CompareTag("FireWizard") ? "IceWizard" : "FireWizard";
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        if (enemies.Length == 0)
            return;

        target = enemies[Random.Range(0, enemies.Length)].transform;

        LookAtTargetInstant();
    }

    void LookAtTargetInstant()
    {
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void MoveTowardsTarget()
    {
        if (isAttacking) return;

        if (GameController.Instance != null && !GameController.Instance.allowWizardMovement)
            return;

        if (animator != null)
        {
            animator.SetBool("isAttack", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
        }

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;

        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void StopMoving()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }

    void StartAttack()
    {
        if (isAttacking) return;

        if (GameController.Instance != null && !GameController.Instance.shouldAllowParticle)
            return;

        if (GameController.Instance != null && !GameController.Instance.allowWizardAttack)
            return;

        isAttacking = true;

        LookAtTargetInstant();

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttack", true);
        }

        SpawnSpell();
    }

    public void SpawnSpell()
    {
        if (target == null) return;

        Vector3 direction = (target.position - spellSpawnPoint.position).normalized;
        direction.y = 0;
        
        Quaternion spellRotation = direction != Vector3.zero 
            ? Quaternion.LookRotation(direction) 
            : transform.rotation;
        
        GameObject spell = Instantiate(spellPrefab, spellSpawnPoint.position, spellRotation);
        
        SpellProjectile spellScript = spell.GetComponent<SpellProjectile>();
        if (spellScript != null)
        {
            spellScript.SetOwnerTag(gameObject.tag);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        if (animator != null)
            animator.SetBool("isAttack", false);
    }
}