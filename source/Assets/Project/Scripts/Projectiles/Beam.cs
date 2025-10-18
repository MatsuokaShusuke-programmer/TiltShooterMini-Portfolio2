using System.Collections;
using UnityEngine;

public class Beam:MonoBehaviour{
    public int attack=1;
    [SerializeField]float _speed=5f;
    [SerializeField]float _destroyTime=10f;
    
    Rigidbody rb;
    
    bool _isUsed=false;//使われたか
    
    void Awake(){
        rb=GetComponent<Rigidbody>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        StartCoroutine(Destroy());
        rb.linearVelocity=transform.forward*_speed;//前に飛ぶ
    }
    
    IEnumerator Destroy(){
        yield return new WaitForSeconds(_destroyTime);
        Destroy(gameObject);
    }
    
    //使えるかどうかを変える
    public bool TryUse(){
        if (_isUsed) return false;//すでに使われている->使えない
        _isUsed=true;//はじめてつかわれた->使える
        return true;
    }
}