#pragma strict

var iniRot: Quaternion;

function Start(){ 
    iniRot = transform.rotation; 
}

function LateUpdate(){ 
    transform.rotation = iniRot; 
}