var delayTime : float;
var destroyTime : float;
private var CreateTime : float;

function Start () 
{
	CreateTime = Time.time;
	OnRenderOnOff ( false );
}

function Update () 
{
	if( CreateTime + delayTime < Time.time )
	{
		OnRenderOnOff ( true );
	}
	
	if( CreateTime + delayTime + destroyTime < Time.time )
	{
		Destroy(gameObject);
	}

}

function OnRenderOnOff ( onoff : boolean )
{
	var render = GetComponentsInChildren ( MeshRenderer );
	
	for ( var ren : MeshRenderer in render )
	{
		ren.enabled = onoff;
	}
}

