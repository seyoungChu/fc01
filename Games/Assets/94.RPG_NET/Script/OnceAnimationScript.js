
var animationTarget : Animation;
var take : AnimationState;
var animationSpeed : float;

function Start ()
{
	animationTarget.wrapMode = WrapMode.Loop;
	animationTarget[ "Take 001" ].wrapMode = WrapMode.ClampForever; //
	
	take = ((animationTarget as Animation)["Take 001"] as AnimationState);
	take.enabled = false;
	take.normalizedTime = 0.0;
	take.speed = animationSpeed;
	take.weight = 2;
	
}
function Update () {
	
	if( take.enabled == false )
	{
		take.enabled = true;
	}
	
	if ( take.normalizedTime > 0.99 )
	{
		Destroy(gameObject);
	}
}