﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineTrigger : MonoBehaviour {

    [SerializeField]
    private int id;
    public List<int> m_idList;
	private LineRenderer m_lineRenderer;
    private SpriteRenderer m_spriteRenderer;
    private AudioSource m_AudioSource;
    private AudioClip sfx_press,sfx_up,sfx_none;
	private Vector3 mousePosition;
	private bool isClick;
    private int lineCount = 100;
    private Vector3 centerPoint; //public test暂代
    
	public GameObject lantern;
    public GameObject[] deco;

    [SerializeField]
    private Color activeColor, inactiveColor;

    private Transform rabbit;
	private Vector3 lineEndPoint;
	private bool lighted;
    private bool once;


	void Awake(){
		m_lineRenderer = GetComponent<LineRenderer> ();
        m_AudioSource = Camera.main.GetComponent<AudioSource>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        rabbit = GameObject.Find("Rabbit").transform;
        m_idList = new List<int>();
        sfx_press = Resources.Load("SFX/Press2") as AudioClip;
        sfx_up = Resources.Load("SFX/Press3") as AudioClip;
        sfx_none = Resources.Load("SFX/Press4") as AudioClip;

	}

	void Start () {
		LineSetup ();
        m_spriteRenderer.color = activeColor;
	}


	void Update () {
        //如这里没有lighted，会出现一个线条偏移bug。因为一个trigger只有一个lineRenderer，所以要想生成两条线，需要两个LineRenderer
        //此处使用一个linerenderer即可
        if (isClick && !lighted) { 
			LineRendererUpdate ();
		} 
		if (!isClick ) {
			ColliderCheck ();
		}
		if (lighted) {
			LineRendererOver ();
           // StartCoroutine(CenterPointAni());
		}
        CheckColor();
    }

    void OnMouseDown(){ 
		//注意不能直接定义鼠标位置和线的位置，因为OnMouseDown只执行一次，且只能在按键范围内出发, 因此只能[判定状态]
        if (!GameManager.instance.CanMove)
        {
            return;
        }
        if (id == GameManager.instance.CurID || GameManager.instance.CurID == 0)
        {
            isClick = true;
            m_AudioSource.PlayOneShot(sfx_press);
        }
        else
        {
            m_AudioSource.PlayOneShot(sfx_none);
        }

    }

    void OnMouseUp(){
        if (!GameManager.instance.CanMove)
        {
            return;
        }
        ColliderPreCheck();
        isClick = false;

        if (lighted && !once)
        {
            Flip();
            StartCoroutine("Rabbit");
            m_AudioSource.PlayOneShot(sfx_up);
            GameManager.instance.SetCanMove(false);
        }
        else
        {
            m_AudioSource.PlayOneShot(sfx_none);
        }
    }

    void LineSetup(){
        centerPoint = Vector3.zero;
        m_lineRenderer.positionCount = lineCount;
		for (int i = 0; i < lineCount; i++) {          
			m_lineRenderer.SetPosition (i, transform.position);
		}
	}

	void LineRendererUpdate(){
		m_lineRenderer.enabled = true;
		mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePosition.z = 0;
        float vecX = transform.position.x + (mousePosition.x - transform.position.x) * 0.5f; //使用加法，而不能直接除以二
        float vecY = (mousePosition.y < transform.position.y)? (mousePosition.y - 2f) : (transform.position.y - 2f);
		centerPoint = new Vector3 (vecX, vecY, 0f);
		for (int i = 0; i < lineCount; i++) {
			Vector3 point = beisaier (transform.position, centerPoint, mousePosition,(float)(i*0.01f));
			m_lineRenderer.SetPosition (i, point);
		}
	}

	void LineRendererOver(){
		m_lineRenderer.enabled = true;
	    float vecX2 = transform.position.x + (mousePosition.x - transform.position.x) * 0.5f; //使用加法，而不能直接除以二
        float vecY2 = (mousePosition.y < transform.position.y) ? (mousePosition.y - 2f) : (transform.position.y - 2f);
        centerPoint = new Vector3 (vecX2, vecY2, 0f);
		for (int i = 0; i < lineCount; i++) {
			Vector3 point = beisaier (transform.position, centerPoint, lineEndPoint, (float)(i*0.01f));
			m_lineRenderer.SetPosition (i, point);
		}
        once = true;
	}

    void Flip(){
        if (IsOdd())
            rabbit.localScale = new Vector3(1, 1, 1);
        else
            rabbit.localScale = new Vector3(-1, 1, 1);
    }

    IEnumerator Rabbit(){
        
        for (int i = 0; i < 100; i++)
        {
            Vector3 pos = m_lineRenderer.GetPosition(i);
            rabbit.position = pos;
            yield return new WaitForSeconds(0.01f);

        }
        yield return new WaitForSeconds(0.1f);
        float x = -rabbit.localScale.x;
        rabbit.localScale = new Vector3(x, 1, 1);
        GameManager.instance.SetCanMove(true);
    }

    /*IEnumerator CenterPointAni(){
        for (int i = 0; i < 100; i++)
        {
            if (i%2 == 0)
                yPos++;
            else
                yPos--;
            yield return null;
        }
    }*/

	private Vector3 beisaier(Vector3 p1, Vector3 p2,Vector3 p3,float t){
		Vector3 f = Vector3.zero;
		float t1 = (1 - t) * (1 - t);
		float t2 = t * (1 - t);
		float t3 = t * t;
		f = p1 * t1 + 2 * t2 * p2 + t3 * p3;
		return f;
	}

	private void LanternSetup(){
		for (int i = 1; i <= 3; i++) {
            Vector3 vec = m_lineRenderer.GetPosition (25*i);
            Instantiate(lantern, new Vector3(vec.x, vec.y, -0.3f), Quaternion.identity);
        }
        for (int i = 1; i <= 4; i++)
        {
            Vector3 vec = m_lineRenderer.GetPosition(20 * i);
            if (IsOdd())
            {
                Instantiate(deco[0], new Vector3(vec.x, vec.y, -0.3f), Quaternion.identity);
            }
            else
            {
                Instantiate(deco[1], new Vector3(vec.x, vec.y, -0.3f), Quaternion.identity);
            }
        }

	}

    private void ColliderPreCheck()
    { 
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(mousePosition, 0.01f);
        bool notSelf = (mousePosition - transform.position).sqrMagnitude < 1f ? false : true;
        if (hitColliders.Length == 0 || !notSelf) {
                m_lineRenderer.enabled = false;
            } 
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].GetComponent<LineTrigger>() && notSelf)
            {
                int num = hitColliders[i].GetComponent<LineTrigger>().GetID();
                bool isSame = IsOdd() == hitColliders[i].GetComponent<LineTrigger>().IsOdd();
                if (m_idList.Contains(num) || isSame)
                {
                    m_lineRenderer.enabled = false;
                }
                else
                {
                    lighted = true;
                }
            }
        }
    }

	private void ColliderCheck(){
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(mousePosition,0.01f);
		bool notSelf = (mousePosition - transform.position).sqrMagnitude < 1f ? false : true;

	    if (hitColliders.Length == 0 || !notSelf) {
	    	m_lineRenderer.enabled = false;
    	} 
		for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders [i].GetComponent<LineTrigger> () && notSelf) {
                int num = hitColliders[i].GetComponent<LineTrigger>().GetID();
                bool isSame = IsOdd() == hitColliders[i].GetComponent<LineTrigger>().IsOdd();
                if (m_idList.Contains(num) || isSame)
                {
                    m_lineRenderer.enabled = false;
                }
                else
                {
                    m_idList.Add(num);
                    hitColliders[i].GetComponent<LineTrigger>().AddID(id);
                    LanternSetup();
                    lighted = true;
                    lineEndPoint = hitColliders[i].transform.position;
                    int tmp = hitColliders[i].GetComponent<LineTrigger>().id;
                    GameManager.instance.SetCurID(tmp);
                }
			} 
		}
	}

    public int GetID(){
        return id;
    }

    public void AddID(int num){
        m_idList.Add(num);
    }

    public bool IsOdd(){
        if (id%2 == 0)
            return false;
        else
            return true;
    }

    public void SetID(int num){
        id = num;
    }

    private void CheckColor(){
        if (GameManager.instance.CanMove)
        {
            m_spriteRenderer.color = activeColor;
        }
        else
        {
            m_spriteRenderer.color = inactiveColor;
        }
    }

	/*
	private void RayCheck(){
		//Ray _ray =Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(mousePosition, -Vector2.up);
		if (hit) {
			print("HIT");
			if (hit.transform.GetComponent<LineTrigger> () && !hit.transform.GetComponent<LineTrigger> ().isClick) {
				print ("has line trigger");
				LanternSetup ();
				lighted = true;
				hit.transform.GetComponent<LineTrigger> ().lighted = true;
				hit.transform.GetComponent<CircleCollider2D> ().enabled = false;
				GetComponent<CircleCollider2D> ().enabled = false;
				//LineRendererUpdate ();
			} 
		} else {
			//m_lineRenderer.enabled = false;
		}
	}*/


}
