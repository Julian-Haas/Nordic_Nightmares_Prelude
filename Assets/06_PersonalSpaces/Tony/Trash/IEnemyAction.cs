using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAction {

    public void AlertPlayerPosition();
    public void LostPlayerPosiiton();

    public void IsMad();

    public void NotMad();

    public void SwitchLocation(Pathways newPath, int index = -1);

    public void CheckHearing(NoiseEmitter emitter);

    public Vector3 GetPosition();

}