using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCandleFrom : MonoBehaviour
{

public  Animator candlemacion;
public  float frameanim;

    void Start()
    {

	candlemacion = this.gameObject.GetComponent<Animator>();

        candlemacion.Play ("Candle_Fire_Animation", 0, frameanim);

        // candlemacion.Play("name", 0, x);
 }
    void Update()
    {
        
    }
}
