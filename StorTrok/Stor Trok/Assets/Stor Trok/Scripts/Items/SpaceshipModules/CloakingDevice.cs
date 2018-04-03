using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game-Items/SpaceshipModules/CloakingDevice")]
public class CloakingDevice : SpaceshipModule {

	public float speed_buff = 1.3f;
	public float damage_buff = 10f;


	public override void animation ()
	{
		Spaceship s = Spaceship.get_spaceship (target_object);

		if (s.is_cloaking) {
			s.decloak ();
		} else{
			s.cloak ();
		}
	}
}
