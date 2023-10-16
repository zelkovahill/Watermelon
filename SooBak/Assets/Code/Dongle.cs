using System.Collections;
using UnityEngine;



public class Dongle : MonoBehaviour
{
    public GameManager manager;
    public ParticleSystem effect;
    public int level;
    public bool isDrag;
    public bool isMerge;
    public bool isAttach;

    private CircleCollider2D circle;
    public Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriterenderer;

    private float deadTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim=GetComponent<Animator>();
        spriterenderer=GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        anim.SetInteger("Level",level);
    }

    private void OnDisable()
    {
        
        // 동글 속성 초기화
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;
        
        // 동글 트랜스폼 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation=Quaternion.identity;
        transform.localScale = Vector3.zero;

        // 동글 물리 초기화
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        circle.enabled = true;

    }


    void Update()
    {
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition);
            // x 축 경계 설정 
            float leftBorder = -5.5f + transform.localScale.x/2f;
            float rightBorder = 5.5f - transform.localScale.x/2f;

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }
            
            mousePos.y = 7f;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
        }
       

    }

    public void Drag()
    {
        isDrag = true;
    }
    
    public void Drap()
    {
        isDrag = false;
        rb.simulated = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        if (isAttach)
        {
            yield break;
        }
        
        isAttach = true;
        manager.SfxPlay(GameManager.Sfx.Attach);
        
        yield return new WaitForSeconds(0.2f);
        
        isAttach = false;

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Dongle")
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();

            if (level == other.level&&!isMerge&&!other.isMerge&&level<7)
            {
                // 나와 상대편 위치 가져오기 
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                // 1. 내가 아래에 있을 때 
                // 2. 동일한 높이일 때, 내가 오른쪽에 있을 때
                if (meY < otherY || (meY == otherY && meX > otherX))
                {
                    //상대방은 숨기기 
                    other.Hide(transform.position);
                    // 나는 레벨업 
                    LevelUp();
                }

            }
        }
    }

    public void Hide(Vector3 targetPos)
    {
        isMerge = true;

        rb.simulated = false;
        circle.enabled = false;

        if (targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }

        StartCoroutine(HideRoutine(targetPos));

    }

    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;

        while (frameCount < 20)
        {
            frameCount++;
            if (targetPos != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos,0.5f);
            }
            else if (targetPos == Vector3.up * 100)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }
            
            yield return null;
        }

        manager.score +=(int)Mathf.Pow(2, level);

        isMerge = false;
        gameObject.SetActive(false);

    }

    void LevelUp()
    {
        isMerge = true;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        StartCoroutine(LevelUpRoutine());

    }

    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        
        anim.SetInteger("Level",level+1);
        EffectPlay();
        manager.SfxPlay(GameManager.Sfx.LevelUp);
        
        yield return new WaitForSeconds(0.3f);
        level++;
        manager.maxLevel = Mathf.Max(level, manager.maxLevel);

        isMerge = false;

    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Finish")
        {
            deadTime += Time.deltaTime;
            if (deadTime > 2)
            {
                spriterenderer.color = new Color(0.9f,0.2f,0.2f);
                ;
            }

            if (deadTime > 5)
            {
                manager.GameOver();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            spriterenderer.color = Color.white;
        }
    }

    void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();

    }
}
