using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UIJoyist : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public GameObject m_center;
    public GameObject m_bg;
    public float direction=0;
    private Vector2 initPos;
    public Vector2 directionVec;
    public Vector2 directionTempVec;
    public int offset=100;
    public bool isDrag = false;
    private void Awake()
    {
        initPos = m_bg.transform.position;
        direction = 0;
        directionVec = Vector3.Normalize(new Vector2(0,0));
        directionTempVec = Vector3.Normalize(new Vector2(0,0));
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        m_bg.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_center.transform.position = eventData.position;
        Vector2 vector2 = m_center.transform.GetComponent<RectTransform>().anchoredPosition;
        float angle = DeltaPos2Angle(vector2.x, vector2.y);
        directionVec = Vector3.Normalize(vector2) ;
        directionTempVec = Vector3.Normalize(vector2) ;
        direction = angle;
        isDrag = true;
    }
    static public float DeltaPos2Angle(float x, float y)
    {
        float fx = x;
        float fy = y;
        if (Mathf.Abs(fy) < 0.0001f)
        {
            if (fx > 0)
            {
                return 90;
            }
            else
            {
                return -90;
            }
        }

        float fRot = Mathf.Rad2Deg * Mathf.Atan(fx / fy);
        if (fy < 0)
        {
            fRot += 180f;
            if (fRot > 180)
                fRot -= 360f;
            //fRot = Mathf.Repeat(fRot, 360f );
        }
        return fRot;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        m_bg.transform.position = initPos;
        m_center.transform.localPosition = new Vector3(0,0,0);
        directionVec = Vector3.Normalize(new Vector2(0, 0));
        //directionTempVec = Vector3.Normalize(new Vector2(0, 0));
        //direction = 0;
        isDrag = false;
    }
}
