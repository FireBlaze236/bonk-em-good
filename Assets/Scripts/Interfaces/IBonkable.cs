using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBonkable
{
    void Bonk(float value, int damage);
    void Hit(float value, int damage);
}
