using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    [Header("--------------[ Core ]")]
    public bool isOver;
    public int score;
    public int maxLevel;

    [Header("--------------[ UI ]")]
    public TextMeshProUGUI scoreText;
    public GameObject GameSet;


    [Header("--------------[ Object Pooling ]")]
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public List<Dongle> donglePool;
    public GameObject effectPrefab;
    public Transform effectGroup;
    public List<ParticleSystem> effectPool;

    [Range(1,30)]
    public int poolSize;
    public int poolCursor;
    public Dongle lastDongle;
    
    [Header("--------------[ Aidio ]")]
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;

    
    
    

    public enum Sfx
    {
        LevelUp,
        Next,
        Attach,
        Button,
        Over,
        Kim,
        A,
        M,
        Sorry,
        H,
        N
    };
    private int sfxCursor;

    
    
    
    void Awake()
    {
        Application.targetFrameRate = 60;

        donglePool = new List<Dongle>();
        effectPool = new List<ParticleSystem>();
        for (int i = 0; i < poolSize; i++)
        {
            MakeDongle();
        }
    }
    
    void Start()
    {
        bgmPlayer.Play();
        NextDongle();
    }

    Dongle MakeDongle()
    {
        // 이펙트 생성
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect " + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);
        
        // 동글 생성
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup);
        instantDongleObj.name = "Dongle " + donglePool.Count;
        Dongle instantDongle = instantDongleObj.GetComponent<Dongle>();
        instantDongle.manager = this;
        instantDongle.effect = instantEffect;
        donglePool.Add(instantDongle);
        
        return instantDongle;
        
    }

    Dongle GetDongle()
    {
        for (int i = 0; i < donglePool.Count; i++)
        {
            poolCursor = (poolCursor + 1) % donglePool.Count;
            if (!donglePool[poolCursor].gameObject.activeSelf)
            {
                return donglePool[poolCursor];
            }
        }

        return MakeDongle();
    }

    void NextDongle()
    {
        if (isOver)
        {
            return;
        }


        lastDongle = GetDongle();
      lastDongle.level = Random.Range(0, maxLevel);
      lastDongle.gameObject.SetActive(true);
      
      
      SfxPlay(GameManager.Sfx.Next);
      StartCoroutine(WaitNext());
    }

    IEnumerator WaitNext()
    {
        while (lastDongle!=null)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(2.5f);

        NextDongle();
    }
  

    public void TouchDown()
    {
        if (lastDongle == null) 
        return;
        
        lastDongle.Drag();
    }

    public void TouchUp()
    {
        if(lastDongle==null)
            return;
        
        lastDongle.Drap();
        lastDongle = null;
    }

    public void GameOver()
    {
        if (isOver)
        {
            return;
        }

        isOver = true;

        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        SfxPlay(Sfx.Over);
        bgmPlayer.Stop();
        // 1. 장면 안에 활성화 되어 있는 모든 동글 가져오기
        Dongle[] dongles = FindObjectsOfType<Dongle>();

        // 2. 지우기 전에 모든 동글의 물리효과 비활성화
        for (int i = 0; i < dongles.Length; i++)
        {
            dongles[i].rb.simulated = false;
        }
        
        // 3.  1번의 목록을 하나씩 접근해서 지우기
        for (int i = 0; i < dongles.Length; i++)
        {
            dongles[i].Hide(Vector3.up*100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(8f);

        GameSet.SetActive(true);
        SfxPlay(Sfx.N);
    }

    public void SfxPlay(Sfx type)
    {
        switch (type)
        {
            case Sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(0, 3)];
                break;
            case Sfx.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            case Sfx.Attach:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            case Sfx.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[5];
                break;
            case Sfx.Over:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;
            case Sfx.Kim:
                sfxPlayer[sfxCursor].clip = sfxClip[7];
                break;
            case Sfx.A:
                sfxPlayer[sfxCursor].clip = sfxClip[8];
                break;
            case Sfx.M:
                sfxPlayer[sfxCursor].clip = sfxClip[9];
                break;
            case Sfx.Sorry:
                sfxPlayer[sfxCursor].clip = sfxClip[10];
                break;
            case Sfx.H:
                sfxPlayer[sfxCursor].clip = sfxClip[11];
                break;
            case Sfx.N:
                sfxPlayer[sfxCursor].clip = sfxClip[12];
                break;

        }
        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

    private void LateUpdate()
    {
        scoreText.text = score.ToString();
    }
}
