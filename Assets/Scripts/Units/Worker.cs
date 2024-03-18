using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField]
    private ResourceSource curResourceSource; //เป้าหมายที่เขาจะทำเช่น ต้นไม้ที่จะตัด ทองที่จะขุด
    public ResourceSource CurResourceSource { get { return curResourceSource; } set { curResourceSource = value; } }

    [SerializeField]
    private float gatherRate = 0.5f; //ความถี่ที่เขาจะตัดไม้/ขุดทอง เช่น ทุก  0.5 วิ
    [SerializeField]
    private int gatherAmount = 1; // An amount unit can gather every "gatherRate" second(s)  //จำนวนที่จะตัด/ขุดได้ในทุก gatherRate เช่น ทุก 0.5 วิ ฟัน 1 โป๊ก ได้ 1 อัน

    [SerializeField]
    private int amountCarry; //amount currently carried จำนวนไม้ทื่ถืออยู่ในตัว
    public int AmountCarry { get { return amountCarry; } set { amountCarry = value; } }

    [SerializeField]
    private int maxCarry = 25; //max amount to carry  จำนวนที่ถือได้สูงสุด
    public int MaxCarry { get { return maxCarry; } set { maxCarry = value; } }

    [SerializeField]
    private ResourceType carryType; //กำลัง carry ทรัพยากรประเภทไหนอยู่
    public ResourceType CarryType { get { return carryType; } set { carryType = value; } }

    private float lastGatherTime;  //จะมีการคำนวณใน codeจะอธิบายอีกที
    private Unit unit; //script unit ที่ต้องโหลดคู่กัน


    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (unit.State)
        {
            case UnitState.MoveToResource:
                MoveToResourceUpdate();
                break;
            case UnitState.Gather:
                GatherUpdate();
                break;
            case UnitState.DeliverToHQ:
                DeliverToHQUpdate();
                break;
            case UnitState.StoreAtHQ:
                StoreAtHQUpdate();
                break;
        }
    }
    // move to a resource and begin to gather it
    public void ToGatherResource(ResourceSource resource, Vector3 pos)
    {
        curResourceSource = resource;

        //if gather a new type of resource, reset amount to 0
        if (curResourceSource.RsrcType != carryType)
        {
            carryType = CurResourceSource.RsrcType;
            amountCarry = 0;
        }

        unit.SetState(UnitState.MoveToResource);

        unit.NavAgent.isStopped = false;
        unit.NavAgent.SetDestination(pos);
    }

    private void MoveToResourceUpdate()
    {
        if (Vector3.Distance(transform.position, unit.NavAgent.destination) <= 2f)
        {
            if (curResourceSource != null)
            {
                unit.LookAt(curResourceSource.transform.position);
                unit.NavAgent.isStopped = true;
                unit.SetState(UnitState.Gather);
            }
        }
    }

    private void GatherUpdate()
    {
        //ถ้าเวลาที่ดำเนินผ่านไป ลบกับ เวลาที่ตัดไม้ก่อนหน้านี้  มากกว่า อัตราความถี่ในการตัดไม้ (=0.5วิ)        
        if (Time.time - lastGatherTime > gatherRate)
        {
            lastGatherTime = Time.time; //ให้เซ้ตตัวแปร เป็นเวลาครั้งสุดท้ายที่ตัดจึ๊ก

            //เริ่มการตัด
            if (amountCarry < maxCarry) //ถ้าปริมาณขอที่ถืออยู่ในมือ ยังไม่เกิน maxที่ถือได้
            {
                if (curResourceSource != null) //ถ้าต้นไม้ยังอยู่ ค่อยตัด
                {
                    curResourceSource.GatherResource(gatherAmount);

                    carryType = curResourceSource.RsrcType;
                    amountCarry += gatherAmount; //ถือเพิ่มตามจำนวนที่ตัดเพิ่ม
                }
            }
            else //amount is full, go back to deliver at HQ  ถ้าเต็มมือแล้ว
                unit.SetState(UnitState.DeliverToHQ);
        }
    }

    private void DeliverToHQUpdate()
    {
        if (Time.time - unit.LastPathUpdateTime > unit.PathUpdateRate)
        {
            unit.LastPathUpdateTime = Time.time;

            unit.NavAgent.SetDestination(unit.Faction.GetHQSpawnPos());
            unit.NavAgent.isStopped = false;
        }

        if (Vector3.Distance(transform.position, unit.Faction.GetHQSpawnPos()) <= 1f)
            unit.SetState(UnitState.StoreAtHQ);
    }

    private void StoreAtHQUpdate()
    {
        unit.LookAt(unit.Faction.GetHQSpawnPos());  //หันไปมอง

        if (amountCarry > 0) //ถ้ามีของให้ส่ง
        {
            // Deliver the resource to Faction
            unit.Faction.GainResource(carryType, amountCarry);
            amountCarry = 0; //ส่งเสร็จแล้ว ให้ของในมือเป็น 0

            //Debug.Log("Delivered");
        }
    }



}

