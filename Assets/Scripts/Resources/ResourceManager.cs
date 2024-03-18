using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] woodTreePrefab;

    [SerializeField]
    private Transform woodTreeParent;
    //สร้าง parent เพื่อให้เมื่อสร้างต้นไม้ในซีนเยอะ ๆ ต้นไมลูกจะมาเกาะ parent นี้ เพื่อไม่ให้รกซีน

    [SerializeField]
    private ResourceSource[] resources; //เก็บ resources ทุกอย่างในซีน

    public static ResourceManager instance;

    void Awake()
    {
        instance = this;
    }



    // Start is called before the first frame update
    void Start()
    {
        FindAllResource();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FindAllResource()
    {
        resources = FindObjectsOfType<ResourceSource>();
    }

}
