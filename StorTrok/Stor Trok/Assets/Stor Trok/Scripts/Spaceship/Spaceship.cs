using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectGroupType{
	None,
	Player,
	Enemy
}

[RequireComponent(typeof(Rigidbody))]
public class Spaceship : MonoBehaviour { // ein spaceship objekt muss auf jedem raumschiff sein

	/// <summary>
	/// Alle aktiven Spaceship-Instanzen in der Scene, die nicht destroyed sind
	/// </summary>
	public static List<Spaceship> active_spaceships = new List<Spaceship> ();

	/// <summary>
	/// Findet die Spaceship-Instanz, die auf dem GameObject oder in einem seiner Parents ist, oder null, wenn diese nicht existiert
	/// </summary>
	/// <returns>Die Spaceship-Instanz oder null.</returns>
	/// <param name="o">Das GameObject, zu dem die Spaceship-Instanz gesucht werden soll.</param>
	public static Spaceship get_spaceship(GameObject o){
		if (o == null)
			return null;
		if (o.GetComponent<Spaceship> () != null)
			return o.GetComponent<Spaceship> ();
		Spaceship[] s = o.GetComponentsInParent<Spaceship> (true);
		return s.Length > 0 ? s [0] : null;
	}
	public static List<Spaceship> get_active_enemy_spaceships(){
		List<Spaceship> sl = new List<Spaceship> ();
		foreach (Spaceship s in Spaceship.active_spaceships){
			if (s.spaceship_group_type == ObjectGroupType.Enemy)
				sl.Add (s);
		}
		return sl;
	}
	public static List<Spaceship> get_active_player_spaceships(){
		List<Spaceship> sl = new List<Spaceship> ();
		foreach (Spaceship s in Spaceship.active_spaceships){
			if (s.spaceship_group_type == ObjectGroupType.Player)
				sl.Add (s);
		}
		return sl;
	}

	/// <summary>
	/// Ein ScriptableObject mit den nötigen Informationen zu diesem Spaceship
	/// </summary>
	public Raumschiff raumschiff;

	/// <summary>
	/// Maximale Angriffsdistanz der Raumschiffe
	/// </summary>
	public static float max_attack_distance = 2000;
	/// <summary>
	/// die Zeit, in der man nicht im Kampf sein darf, um den "out-of-combat" status zu erhalten
	/// </summary>
	public const float min_out_of_combat_time = 5;
	/// <summary>
	/// Gibt den maximalen Drehwinkel an, den ein nicht-Spieler-Raumschiff in der lokalen x-Achse haben kann
	/// </summary>
	public const float max_x_rotation = 65;

	/// <summary>
	/// Die Gruppe, in der diese Spaceship-Instanz drin ist (Spieler/Gegner)
	/// </summary>
	public ObjectGroupType spaceship_group_type = ObjectGroupType.None;
	/// <summary>
	/// Gibt an, ob die Spaceship stats (Geschwindigkeit etc.) von den Items abhängen und nicht von den Werten, die im Inspector zugeordnet wurden. (
	/// Wurde früher für Test-Zwecke benutzt, jetzt aber nicht mehr.
	/// </summary>
	public bool spaceship_stats_depend_on_items = true;

	/// <summary>
	/// Eine Schild-Instanz, die die Informationen bezüglich Schildstärke etc. beinhaltet.
	/// </summary>
	public Schild schild; // schild instanz für diese spaceship instanz

	/// <summary>
	/// Wenn das Raumschiff am Anfang in die Szene reinwarpen sollte, gibt dieses GameObject die Position des Punktes an, an dem der Warpflug beginnt
	/// </summary>
	public GameObject warp_in_startpoint;
	/// <summary>
	/// GFX hinter dem Raumschiff bei Warp (wird nicht mehr benutzt)
	/// </summary>
	public GameObject warpspur;
	/// <summary>
	/// GFX für die Impulsantriebspur
	/// </summary>
	public GameObject auspuff;

	/// <summary>
	/// Bestimmt, wie stark das Spaceship bei Zerstörung verformt wird
	/// </summary>
	public float destroyed_deform_factor = 1;

	/// <summary>
	/// Der Zeitpunkt, an dem das Raumschiff das letzte Mal angegriffen hat
	/// </summary>
	float last_time_attack = -10;
	/// <summary>
	/// Der Zeitpunkt, an dem das Raumschiff das letzte Mal Schaden genommen hat
	/// </summary>
	float last_time_dmg_received = -10;

	/// <summary>
	/// Gibt an, ob das Raumschiff zerstört ist.
	/// </summary>
	[HideInInspector]
	public bool destroyed = false;

	/// <summary>
	/// Die Status-Effekte, die auf dem Schiff sind. (Buffs/Debuffs)
	/// </summary>
	public List<StatusEffect> status_effects;

	/// <summary>
	/// Gibt an, ob das Raumschiff zerstörbar ist. (Nur für Testzwecke)
	/// </summary>
	public bool immortal = false;

	/// <summary>
	/// Die maximale Hüllenstärke
	/// </summary>
	[HideInInspector] public float max_structural_integrity = 100;
	/// <summary>
	/// Die aktuelle Hüllenstärke
	/// </summary>
	public float structural_integrity = 0;

	/// <summary>
	/// Maximale Schildstärke
	/// </summary>
	[HideInInspector]
	public float max_total_shields = 0;

	/// <summary>
	/// Base Schaden Output, der durch Buffs/Debuffs beinflusst werden kann
	/// </summary>
	float damage_base_output = 1;

	/// <summary>
	/// Gibt an, wie ungenau die Phaser schießen
	/// </summary>
	float phaser_fail_tolerance = 2.5f;

	/// <summary>
	/// Gibt an, ob für dieses Spaceship schon ein Icon auf der Minimap erstellt wurde
	/// </summary>
	[HideInInspector] public bool minimap_icon_created = false;

	/// <summary>
	/// Maximale Base Geschwindigkeit, abhängig von dem Impuls-Antrieb Item
	/// </summary>
	float max_base_speed;

	/// <summary>
	/// Eingestellte Geschwindigkeit des Raumschiffs, z.B durch den Spieler-Input (E/Q), zwischen -1 und 4
	/// </summary>
	[HideInInspector]
	public float speed; // -1 <= speed <= 4
	/// <summary>
	/// Gibt an, wie stark das Raumschiff beschleunigen kann
	/// </summary>
	[HideInInspector]
	public float accel;

	/// <summary>
	/// Basis Drehgeschwindigkeit
	/// </summary>
	[HideInInspector]
	public float base_rotation_speed;
	/// <summary>
	/// Gibt an, wie stark sich das Raumschiff bei Links/Rechts-Drehung um die lokale z-Achse (vorwärts-Achse) dreht
	/// </summary>
	public const float rotation_z_axis_tilt_amount = 20;

	//Items
	/// <summary>
	/// Die Waffenpositionen des Raumschiffs, also die Orte, von denen Phaser, Torpedos etc. abgefeuert werden können
	/// </summary>
	public List<SpaceshipWeaponPosition> spaceshipWeaponPositions;

	/// <summary>
	/// Das Impuls-Antriebs-Item, das dem Schiff zugewiesen ist
	/// </summary>
	[HideInInspector]
	public ImpulsAntrieb impuls_antrieb;
	/// <summary>
	/// Das Warp-Antriebs-Item, das dem Schiff zugewiesen ist
	/// </summary>
	[HideInInspector]
	public WarpAntrieb warp_antrieb;

	/// <summary>
	/// Der Antrieb, der zurzeit benutzt wird (Warp/Impuls)
	/// </summary>
	public AntriebType antrieb_type = AntriebType.Impuls;

	/// <summary>
	/// Die Value-Holder Variable für das Schild-Item
	/// </summary>
	private SchildItem schild_item_value;
	public SchildItem schild_item{
		get{ return schild_item_value; }
		set{ 
			if (schild_item_value == null) {
				schild_item_value = value;
				apply_item_stats ();
				create_shield ();
			} else {
				schild_item_value = value;
			}
		}
	}

	/// <summary>
	/// Die Raumschiffe-Module, mit denen dieses Raumschiff ausgestattet ist
	/// </summary>
	[HideInInspector]
	public List<SpaceshipModule> modules = new List<SpaceshipModule>();



	/// <summary>
	/// Durchsichtigkeit des Raumschiffs bei Tarnung
	/// </summary>
	public const float cloak_effect_alpha = 0.4f;

	/// <summary>
	/// Gibt an, ob sich das Raumschiff gerade tarnt
	/// </summary>
	//[HideInInspector]
	public bool is_cloaking = false;

	/// <summary>
	/// Das GameObject, das für die Schild-Schild-Graphik und die Collision zuständig ist
	/// </summary>
	public GameObject schild_collision_object;
	/// <summary>
	/// Das GameObject, das die GFX beinhaltet (häufig das Object selbst)
	/// </summary>
	public GameObject gfx_parent;


	/// <summary>
	/// Die vier GameObjects der Viertelkreise, die die jeweilige Schildstärke angeben
	/// </summary>
	[Header("Schild-Zustand Anzeiger")]
	public GameObject shields;
	private GameObject shield_top;
	private GameObject shield_left;
	private GameObject shield_right;
	private GameObject shield_bot;

	/// <summary>
	/// Gibt an, ob das Raumschiff Schild-Anzeiger hat (die vier Viertelkreise)
	/// </summary>
	public bool has_shield_indicator = true;

	/// <summary>
	/// Die Farben für den Schild-Anzeiger
	/// </summary>
	Color shield_full_color = new Color (0, 0, 1, 0.6f);
	Color shield_middle_color = new Color (1, 0.9f, 0, 0.6f);
	Color shield_down_color = new Color (1, 0, 0, 0.6f);

	//public Gradient shield_color_gradient;

	/// <summary>
	/// Gibt an, wie lange das Raumschiff braucht, um am Anfang reinzuwarpen
	/// </summary>
	float warp_in_time = 1.5f;

	/// <summary>
	/// Gibt an, ob das Raumschiff gerade am Reinwarpen ist
	/// </summary>
	[HideInInspector] public bool warping_in = false;

	/// <summary>
	/// Startposition vor dem Reinwarpen, also Ziel des Warps
	/// </summary>
	[HideInInspector] public Vector3 start_pos;
	/// <summary>
	/// Startposition des Reinwarpens
	/// </summary>
	[HideInInspector] public Vector3 warpin_start_pos;

	/// <summary>
	/// Gibt an, ob das Raumschiff gerade den Auto-Pilot an hat
	/// </summary>
	[HideInInspector] public bool is_auto_navigating;

	/// <summary>
	/// Zielobjekt des Auto-Piloten
	/// </summary>
	[HideInInspector] public GameObject auto_navigation_target_object;
	/// <summary>
	/// Zielpunkt des Auto-Piloten
	/// </summary>
	private Vector3 auto_navigation_target_point = Vector3.zero;
	/// <summary>
	/// Gibt an, wie stark die Abweichung beim Geradeaus-Fliegen des Auto-Piloten maximal sein darf
	/// </summary>
	[HideInInspector] float min_navigation_angle = 3;

	/// <summary>
	/// Gibt an, wie nah das Raumschiff am Ziel dran sein muss, um den Auto-Piloten abzuschalten
	/// </summary>
	public const float auto_navigation_target_distance = 60;

	/// <summary>
	/// Normale Reisegeschwindigkeit des Auto-Piloten
	/// </summary>
	[HideInInspector] public float auto_navigation_target_speed = 4;
	/// <summary>
	/// Sollte das Raumschiff in die entgegengesetzte Richtung fliegen, muss eine neue Achse für die Rotation hinzugefügt werden.
	/// Aufgrund der Behebung eines anderen Fehlers tritt dies nun aber nicht mehr ein.
	/// (Genaugenommen ist dies kein Bug, sondern nur eine logische Konzequenz des Kreuzproduktes. Da dies aber zuerst wie ein Bug aussah, heißt es immer noch Bug)
	/// </summary>
	[HideInInspector] public Vector3 auto_navigation_bug_180_correction_axis = new Vector3 (0, 1, 0);

	/// <summary>
	/// Gibt an, ob das Raumschiff eine Ziel-Geschwindigkeit hat, auf die es kommen muss
	/// </summary>
	private bool has_target_speed = false;
	/// <summary>
	/// Zielgeschwindigkeit
	/// </summary>
	private float target_speed = 0;

	/// <summary>
	/// Der Rigidbody des Raumschiffs
	/// </summary>
	private new Rigidbody rigidbody;

	/// <summary>
	/// Hauptrenderer des Raumschiffs
	/// </summary>
	private new Renderer renderer;

	/// <summary>
	/// Hauptmaterial des Raumschiffs
	/// </summary>
	private Material material;


	/// <summary>
	/// Coroutine für eine Phaser-Salve
	/// </summary>
	private IEnumerator phaser_salve_coroutine;

	/// <summary>
	/// Funktion für einen Phaser-Salven-Buff.
	/// Es werden in regelmäßigen Zeitintervallen neue Phaser abgefeuert
	/// </summary>
	/// <param name="time_delay">Das Zeitintervall.</param>
	/// <param name="salve_size">Die Größe der Salve, also wie viele Phaser abgefeuert werden sollen.</param>
	/// <param name="weapon">Der spezielle Phasertyp.</param>
	/// <param name="p_enemy">Das Zielobjekt für den Phaser.</param>
	/// <param name="layer">Eine Layer-Mask für den Phaser.</param>
	/// <param name="effect">Der Phaser-Salven-Effekt.</param>
	IEnumerator phaser_salve_function(WaitForSeconds time_delay, int salve_size, Weapon weapon, GameObject p_enemy, int layer, WeaponEffect effect){
		for (int i = 0; i < salve_size; i++) {
			yield return time_delay;
			if (p_enemy.is_destroyed ()) {
				yield break;
			} else {
				fire_subsequent_phaser (weapon, p_enemy, layer, effect);
			}
		}
	}

	/// <summary>
	/// Wird zu Beginn der Scene aufgerufen und initialisiert das Raumschiff (Item-Werte etc.)
	/// </summary>
	void Awake () {
		status_effects = new List<StatusEffect> ();

		// falls das gfx_parent-objekt noch nicht im inspector festgelegt wurde, wird das object selbst genommen, sofern es einen Renderer hat
		if (gfx_parent == null) {
			if (GetComponent<Renderer> () != null) {
				gfx_parent = gameObject;
			} else {
				print ("Error no gfx object");
			}
		}

		// Die Startposition des Raumschiff ist der Ort, an dem es am Anfang ist
		start_pos = transform.position;

		rigidbody = GetComponent<Rigidbody> ();
		renderer = gfx_parent == this ? GetComponent<Renderer>() : gfx_parent.GetComponentInChildren<Renderer> ();
		material = renderer.material;

		setup_raumschiff_stats ();
		if (!enabled)
			return;

		if (warp_in_startpoint) {
			warpin_start_pos = warp_in_startpoint.transform.position;
		}
		if (warpspur != null) {
			warpspur.SetActive (false);
		}

		if (auspuff != null) {
			auspuff.SetActive (false);
		}

		if (raumschiff.raumschiff_type == AlleRaumschiffe.None) {
			print ("set raumschifftype for " + this.name);
		}
		if (raumschiff.raw_raumschiff_type == RawRaumschiffType.None || raumschiff.raw_raumschiff_type == RawRaumschiffType.All) {
			print ("set raw_raumschiff_type for " + this.name);
		}

		if (this.spaceship_group_type == ObjectGroupType.None) {
			print ("set raumschiff_group_type for " + this.name);
			switch (gameObject.layer) {
			case (Player.player_layer):
				print ("Player group type set for " + this.name);
				this.spaceship_group_type = ObjectGroupType.Player;
				break;
			case (Player.enemy_layer):
				print ("enemy group type set for " + this.name);
				this.spaceship_group_type = ObjectGroupType.Enemy;
				break;

			}
		}

		apply_item_stats();

		create_shield ();
		if (has_shield_indicator) {
			setup_shield_gos ();
		}

		if (accel == 0) {
			print ("Warning: accel of " + name + " is 0");
		}
		if (base_rotation_speed == 0) {
			print ("Warning: rotation_speed of " + name + " is 0");
		}

		if (spaceshipWeaponPositions == null)
			spaceshipWeaponPositions = new List<SpaceshipWeaponPosition> ();

		SpaceshipWeaponPosition[] weapon_positions = GetComponentsInChildren<SpaceshipWeaponPosition> ();
		foreach (SpaceshipWeaponPosition p in weapon_positions) {
			if (!spaceshipWeaponPositions.Contains (p))
				spaceshipWeaponPositions.Add (p);
		}
		set_layer (spaceship_group_type == ObjectGroupType.Enemy ? Player.enemy_layer : Player.player_layer);
	}

	/// <summary>
	/// Legt bestimmte Werte anhand der Informationen aus dem raumschiff-ScriptableObject fest
	/// </summary>
	void setup_raumschiff_stats(){
		max_structural_integrity = raumschiff.structural_integrity;
		structural_integrity = raumschiff.structural_integrity;

		base_rotation_speed = raumschiff.rotation_speed;
		accel = raumschiff.acceleration;
	}

	/// <summary>
	/// Legt die Items fest, die standardmäßig auf dem Raumschiff vorhanden sind.
	/// (Das gilt nicht für den Spieler selbst, da dieser auch eigene Item-Kombinationen auf seinem Raumschiff haben kann.)
	/// </summary>
	public void setup_standard_items(){
		if (Player.player.spaceship != this) {
			schild_item = raumschiff.standard_schild;
			impuls_antrieb = raumschiff.standard_impuls_antrieb;
			warp_antrieb = raumschiff.standard_warp_antrieb;
		}
	}

	/// <summary>
	///  Setzt die Schild- und Geschwindigkeitswerte den Items entsprechend fest
	/// </summary>
	public void apply_item_stats(){
		if (spaceship_stats_depend_on_items) {
			this.max_base_speed = this.antrieb_type == AntriebType.Warp ? (Utils.item_is_null(this.warp_antrieb) ? 0 : this.warp_antrieb.max_speed): (Utils.item_is_null(this.impuls_antrieb) ? 0 : this.impuls_antrieb.max_speed);
			this.max_total_shields = Utils.item_is_null(this.schild_item) ? 0 : this.schild_item.max_shield_intenstiy * shield_modifier;
			if (this.schild != null) {
				//this.schild.new_power (this.max_total_shields);
				schild.change_max_power(this.max_total_shields);
			}
		}
	}

	/// <summary>
	/// Neues Schild-Item für dieses Raumschiff, z.B. wenn der Spieler eins aus seinem Inventar in den Schild-Item-Slot zieht
	/// </summary>
	/// <param name="schild_neu">Schild neu.</param>
	public void neues_schild(SchildItem schild_neu){
		schild_item = schild_neu;
		apply_item_stats ();
	}

	/// <summary>
	/// Gibt eine Liste aller Items zurück, die zurzeit auf dem Raumschiff sind
	/// </summary>
	/// <value>Die ausgerüsteten Items</value>
	public List<Item> equipped_items{
		get{ 
			List<Item> l = new List<Item> ();
			/*if (impuls_antrieb != null)
				l.Add (impuls_antrieb);
			if (warp_antrieb != null)
				l.Add (warp_antrieb);*/
			if (schild_item != null)
				l.Add (schild_item);
			foreach (SpaceshipWeaponPosition wpos in spaceshipWeaponPositions) {
				foreach (Weapon weapon in wpos.weapons) {
					l.Add (weapon);
				}
			}

			return l;
		}
	}

	/// <summary>
	/// Wenn das Raumschiff enabled wird, wird es zur Liste der aktiven Raumschiffe hinzugefügt
	/// </summary>
	void OnEnable(){
		active_spaceships.Add (this);
	}
	/// <summary>
	/// Wenn das Raumschiff disabled wird, wird es von der Liste der aktiven Raumschiffe entfernt
	/// </summary>
	void OnDisable(){
		active_spaceships.Remove (this);
	}
	/// <summary>
	/// Wenn das Raumschiff zerstört wird, wird es von der Liste der aktiven Raumschiffe entfernt
	/// </summary>
	void OnDestroy(){
		active_spaceships.Remove (this);
	}

	/// <summary>
	/// Setzt für das Object selbst und das Collision Object das Layer fest
	/// </summary>
	/// <param name="n_layer">Das festzulegende Layer</param>
	public void set_layer(int n_layer){
		gameObject.layer = n_layer;
		schild_collision_object.layer = n_layer;
	}

	//Status effects (buffs/debuffs)

	/// <summary>
	/// Gibt die maximal erreichbare Geschwindigkeit zurück.
	/// Diese ist abhängig von der Basis-Geschwindigkeit, die durch das Impuls-Antrieb-Item festgelegt ist, und den Statuseffekten (Buffs/Debuffs), die auf dem Raumschiff sind
	/// </summary>
	/// <value>Die Maximalgeschwindigkeit</value>
	public float max_speed{
		get{ 
			//print(StatusEffect.complete_effects_intensity(EffectTypes.Speed, status_effects));
			return max_base_speed * StatusEffect.complete_effects_intensity(EffectTypes.Speed, status_effects);
		}
	}

	/// <summary>
	/// Gibt die maximal erreichbare Drehgeschwindigkeit zurück.
	/// Diese ist abhängig von der Basis-Dreheschwindigkeit und den Statuseffekten (Buffs/Debuffs), die auf dem Raumschiff sind
	/// </summary>
	/// <value>Die Maximaldrehgeschwindigkeit</value>
	public float rotation_speed{
		get{ 
			return base_rotation_speed * StatusEffect.complete_effects_intensity (EffectTypes.RotationSpeed, status_effects);
		}
	}
	/// <summary>
	/// Gibt einen Schadens-Multiplier zurück
	/// Dieser ist abhängig von den Statuseffekten (Buffs/Debuffs), die auf dem Raumschiff sind
	/// </summary>
	/// <value>Der Schadens-Multiplier</value>
	public float damage_output{
		get{ 
			return damage_base_output * StatusEffect.complete_effects_intensity(EffectTypes.DamageOutput, status_effects);
		}
	}
	/// <summary>
	/// Gibt einen Schild-Multiplier zurück
	/// Dieser ist abhängig von den Statuseffekten (Buffs/Debuffs), die auf dem Raumschiff sind
	/// </summary>
	/// <value>Der Schild-Multiplier</value>
	public float shield_modifier{
		get{ 
			return StatusEffect.complete_effects_intensity (EffectTypes.Schilde, status_effects);
		}
	}

	/// <summary>
	/// Fügt einen neuen StatusEffect zu dem Raumschiff hinzu
	/// </summary>
	/// <param name="ef">Der StatusEffect.</param>
	public void apply_statuseffect(StatusEffect ef){
		ef.start_time = Time.time; // Gibt an, wann der StatusEffect begonnen hat
		this.status_effects.Add (ef);
		Invoke ("check_status_effects", ef.duration); // nach der Dauer des StatusEffects wird diese Methode aufgerufen, um ihn zu entfernen
		if (this == Player.player.spaceship) // sofern dies das Spieler-Raumschiff ist, wrid noch eine Nachricht auf dem Bildschirm ausgegeben
			StatusTexts.status_texts.new_text (ef.name);
		apply_item_stats ();
	}

	/// <summary>
	/// Fügt mehrere StatusEffects hinzu
	/// </summary>
	/// <param name="effects">Die StatusEffects.</param>
	public void apply_statuseffects(StatusEffect[] effects){
		foreach (StatusEffect ef in effects) {
			apply_statuseffect (ef);
		}
	}
	/// <summary>
	/// Entfernt einen speziellen StatusEffect
	/// </summary>
	/// <param name="ef">Der StatusEffect.</param>
	public void remove_statuseffect(StatusEffect ef){
		status_effects.Remove (ef);
	}
	/// <summary>
	/// Entfernt mehrere StatusEffects
	/// </summary>
	/// <param name="effects">Die StatusEffects.</param>
	public void remove_statuseffects(StatusEffect[] effects){
		foreach (StatusEffect ef in effects) {
			remove_statuseffect (ef);
		}
		apply_item_stats ();
	}

	/// <summary>
	/// Überprüft, ob StatusEffects abgelaufen sind und daher entfernt werden müssen
	/// </summary>
	void check_status_effects(){
		List<StatusEffect> rem = new List<StatusEffect> ();
		foreach (StatusEffect e in status_effects) {
			if (e.GetType () == typeof(WeaponEffect)) { // Bei WeaponEffects gibt es eine Ausnahme, da diese nicht direkt nach dem Hinzufügen zu dem Schiff aktiviert werden, sondern erst 
														// beim ersten Mal Feuern mit diesem Effect
				if (!((WeaponEffect)e).enabled)
					continue;
			}
			float diff = Time.time - e.start_time;
			if (Mathf.Abs (diff - e.duration) < 0.1f) {
				rem.Add (e);
			}
		}
		remove_statuseffects (rem.ToArray ());
	}

	/// <summary>
	/// Gibt eine Liste aller WeaponEffects auf dem Raumschiff zurück
	/// </summary>
	/// <returns>Die WeaponEffects.</returns>
	List<WeaponEffect> get_weapon_effects(){
		List<WeaponEffect> effects = new List<WeaponEffect> ();
		foreach (StatusEffect ef in status_effects) {
			if (ef.GetType () == typeof(WeaponEffect)) {
				effects.Add ((WeaponEffect)ef);
			}
		}
		return effects;
	}
	/// <summary>
	/// Überprüft, ob auf dem Raumschiff ein spezieller WeaponEffect ist, und gibt diesen zurück
	/// </summary>
	/// <returns>Einen WeaponEffect vom Typ effect_type oder null.</returns>
	/// <param name="effect_type">Der Typ des WeaponEffects.</param>
	WeaponEffect has_weapon_effect(WeaponEffectTypes effect_type){
		List<WeaponEffect> effects = get_weapon_effects ();
		foreach (WeaponEffect ef in effects) {
			if (ef.weapon_effect_type == effect_type)
				return ef;
		}
		return null;
	}

	// Input für ComputerPlayer

	/// <summary>
	/// Führt einen gegebenen Rotation-Input aus
	/// </summary>
	/// <param name="key">Gedrückter key.</param>
	public void rotation_input(MovementInputKeys key){
		switch (key) {
		case MovementInputKeys.A:
			this.transform.Rotate (new Vector3 (0, -rotation_speed * Time.deltaTime, 0));
			break;
		case MovementInputKeys.S:
			this.transform.Rotate (new Vector3 (-rotation_speed * Time.deltaTime, 0, 0));
			break;
		case MovementInputKeys.D:
			this.transform.Rotate (new Vector3 (0, rotation_speed * Time.deltaTime, 0));
			break;
		case MovementInputKeys.W:
			this.transform.Rotate (new Vector3 (rotation_speed * Time.deltaTime, 0, 0));
			break;
		default:
			print (key.ToString () + " no rotation input");
			break;
		}
	}
	/// <summary>
	/// Führt einen gegebenen Speed-Input aus
	/// </summary>
	/// <param name="key">Gedrückter key.</param>
	public void speed_input(MovementInputKeys key){
		switch (key) {
		case MovementInputKeys.E:
			speed += accel * Time.deltaTime;
			break;
		case MovementInputKeys.Q:
			speed -= accel * Time.deltaTime;
			break;
		}
	}

	/// <summary>
	/// Nur für einige ComputerPlayer
	/// Da das Raumschiff sich in lokalen-Achse maximal um einen bestimmten Winkel drehen darf und in der lokalen z-Achse gar nicht, wird hier daher die Rotation angepasst
	/// </summary>
	public void fix_rotation(){
		if (this == Player.player.spaceship)
			print ("fix rotation called on player spaceship");
		fix_z_rotation ();
		fix_x_rotation ();
	}

	/// <summary>
	/// Passt die lokale z-Rotation an
	/// </summary>
	public void fix_z_rotation(){
		Quaternion rot = transform.localRotation;
		float z = rot.eulerAngles.z;
		if (Mathf.Abs (z - 360) < Mathf.Abs (z)) {
			z = Mathf.Lerp (rot.eulerAngles.z, 360, Time.deltaTime);
		} else {
			z = Mathf.Lerp (rot.eulerAngles.z, 0, Time.deltaTime);
		}
		rot.eulerAngles = new Vector3 (rot.eulerAngles.x, rot.eulerAngles.y, z);
		transform.localRotation = rot;
	}

	/// <summary>
	/// Passt die lokale-x-Rotation an
	/// </summary>
	public void fix_x_rotation(){
		Quaternion rot = transform.localRotation;
		float x = rot.eulerAngles.x;
		if (Mathf.Abs (x - 360) < Mathf.Abs (x)) {
			x = Mathf.Clamp (x, 360-max_x_rotation, 360);
		} else {
			x = Mathf.Clamp (x, 0, max_x_rotation);
		}

		rot.eulerAngles = new Vector3 (x, rot.eulerAngles.y, rot.eulerAngles.z);
		transform.localRotation = rot;
	}

	/// <summary>
	/// Dreht das Raumschiff nach links
	/// </summary>
	public void rotate_left(){
		_rotate (-1);
	}
	/// <summary>
	/// Dreht das Raumschiff nach rechts
	/// </summary>
	public void rotate_right(){
		_rotate (1);
	}/// <summary>
	/// Dreht das Raumschiff nach unten
	/// </summary>
	public void rotate_down(){
		this.transform.Rotate (new Vector3 (rotation_speed * Time.deltaTime, 0, 0), Space.Self);
	}
	/// <summary>
	/// Dreht das Raumschiff nach oben
	/// </summary>
	public void rotate_up(){
		this.transform.Rotate (new Vector3 (-rotation_speed * Time.deltaTime, 0, 0), Space.Self);
	}

	/// <summary>
	/// Dreht das Raumschiff nach links oder rechts, je nach dir
	/// </summary>
	/// <param name="dir">Richtung.</param>
	void _rotate(int dir){
		this.transform.Rotate (new Vector3 (0, -dir * rotation_speed * Time.deltaTime, 0), Space.World);
		Quaternion rot = transform.rotation;
		float z = rot.eulerAngles.z;
		z = dir * rotation_z_axis_tilt_amount;
		rot.eulerAngles = new Vector3 (rot.eulerAngles.x, rot.eulerAngles.y, z);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, rot, Time.deltaTime);
	}
		
	/// <summary>
	/// Erstellt eine Liste alle Waffen auf dem Raumschiff, die theoretisch vom Winkel her gesehen auf den Gegner schießen könnten und gibt diese zurück
	/// </summary>
	/// <returns>Die möglichen Waffen</returns>
	/// <param name="enemy">Der Gegner.</param>
	public List<Weapon> get_possible_weapons(GameObject enemy){
		List<Weapon> l = new List<Weapon> ();

		foreach (SpaceshipWeaponPosition spaceshipWeaponPosition in spaceshipWeaponPositions) {
			Vector3 enemy_to_pos_dir = enemy.transform.position - spaceshipWeaponPosition.transform.position;
			float angle = Vector3.Angle (enemy_to_pos_dir, transform.TransformDirection (spaceshipWeaponPosition.local_direction));
			foreach (Weapon weapon in spaceshipWeaponPosition.weapons) {
				if (angle < weapon.arc_range / 2) {
					l.Add (weapon);
				}
			}
		}
		return l;
	}

	// ((!!!!!!!!!! Careful: Only completly valid for spaceships with only top and bot weaponpositions))

	/// <summary>
	/// Gibt die SpaceshipWeaponPosition zurück, die vorne ist und in die vordere Richtung zeigt
	/// </summary>
	/// <returns>Die vordere SpaceshipWeaponPosition </returns>
	public SpaceshipWeaponPosition get_top_weapon_position(){
		foreach (SpaceshipWeaponPosition wpos in spaceshipWeaponPositions) {
			if (wpos.local_direction.normalized == new Vector3 (0, 0, 1)) {
				//Debug.Log (wpos.gameObject.name);
				return wpos;
			}
		}
		return null;
	}

	/// <summary>
	/// Gibt die SpaceshipWeaponPosition zurück, die hinten ist und in die hintere Richtung zeigt
	/// </summary>
	/// <returns>Die hintere SpaceshipWeaponPosition </returns>
	public SpaceshipWeaponPosition get_bot_weapon_position(){
		foreach (SpaceshipWeaponPosition wpos in spaceshipWeaponPositions) {
			if (wpos.local_direction.normalized == new Vector3 (0, 0, -1)) {
				return wpos;
			}
		}
		return null;
	}

	// ----------------
	/// <summary>
	/// Legt die Viertelkreisobjekte für den Schild-Anzeiger fest
	/// </summary>
	public void setup_shield_gos(){
		if (!shield_top) {
			shield_top = Utils.get_gameobject_in_children(gameObject, "shield_quarter_top");
		}
		if (!shield_left) {
			shield_left = Utils.get_gameobject_in_children(gameObject, "shield_quarter_left");
		}
		if (!shield_bot) {
			shield_bot = Utils.get_gameobject_in_children (gameObject, "shield_quarter_bot");
		}
		if (!shield_right) {
			shield_right = Utils.get_gameobject_in_children(gameObject, "shield_quarter_right");
		}
	}

	/// <summary>
	/// Erstellt eine neue Schild-Instanz für dieses Raumschiff
	/// </summary>
	public void create_shield(){
		schild = new Schild (max_total_shields, gameObject);
	}

	/// <summary>
	/// Lässt das Raumschiff reinwarpen
	/// </summary>
	public void warp_in(){
		warping_in = true;
		this.transform.position = warpin_start_pos;//warping_startpoint.transform.position;
	//	warpspur.SetActive (true);
		if (auspuff!=null)
			auspuff.SetActive (false);
		transform.LookAt (start_pos);
	}

	/// <summary>
	/// Update wird jeden Frame aufgerufen
	/// </summary>
	void Update () {
		if (Player.player == null)
			return;
		if (Player.player.player_environment_status == PlayerEnvironmentStatus.Ground) {
			this.enabled = false;
			return;
		}
		//rigidbody.velocity = speed*transform.forward*max_speed/4;
		rigidbody.MovePosition(rigidbody.position + speed*transform.forward*max_speed/4*Time.deltaTime); // Bewegung des GameObjects abhängig von der Maximalgeschwindigkeit und speed-Input

		if (warping_in) { // sollte das Raumschiff am Reinwarpen sein, wird die warp-methode aufgerufen
			warpin_update ();

		} 

		// legt die Farben der Viertelkreise dem Schild entsprechend fest
		if (has_shield_indicator) {
			set_shield_indicator_color ();
		}

		// wenn der Auto-Pilot aktiv ist, wird dieser ausgeführt
		if (is_auto_navigating) {
			auto_navigate ();
		}
		// Hat das Raumschiff eine Zielgeschwindigkeit, wird sich dieser angenähert
		if (has_target_speed)
			change_speed ();
		
		// Schild und Hülle regenerieren
		regenerate_defense ();

		// Sollte dies das Spieler-Raumschiff sein, wird die jetzigen Spieler-Korrdinaten festgelegt
		if (this == Player.player.spaceship)
			Player.player.last_coordinate = transform.position;
	}

	/// <summary>
	/// Reinwarpen
	/// </summary>
	void warpin_update(){
		float dist_to_warp_start = Vector3.Distance (transform.position, warpin_start_pos);
		float max_dist = Vector3.Distance (start_pos, warpin_start_pos);

		float perc = dist_to_warp_start / max_dist;

		float time = perc;

		float new_pos_perc = time + Time.deltaTime / warp_in_time;

		Vector3 new_pos = Vector3.Lerp (warpin_start_pos, start_pos, new_pos_perc);
		transform.position = new_pos;

		if (new_pos_perc >= 1) {
			warping_in = false;
			Invoke ("start_mission", 2);
		}
	}

	/// <summary>
	/// Regeneriert Hülle/Schild in Abhängigkeit vom in_battle-Status und von den Regenerationswerten
	/// </summary>
	void regenerate_defense(){

		float in_battle_modifier = 0.1f;

		float schild_regen = schild_item == null ? 0 : schild_item.shield_regeneration_rate * Time.deltaTime;
		float si_regen = raumschiff.structural_integrity_regeneration_rate * Time.deltaTime;
		if (in_battle) {
			schild_regen *= in_battle_modifier;
			si_regen *= in_battle_modifier;
		}
		if (schild_item!=null)
			schild.regenerate_shield (schild_regen);
		structural_integrity += si_regen;
		structural_integrity = Mathf.Min (raumschiff.structural_integrity, structural_integrity);

	}

	/// <summary>
	/// Nähert sich der Zielgeschwindigkeit
	/// </summary>
	void change_speed(){
		if (Mathf.Abs (speed - target_speed) <= Time.deltaTime * accel) {
			speed = target_speed;
			has_target_speed = false;
		} else {
			speed += Time.deltaTime * accel * (target_speed < speed ? -1 : 1);
			has_target_speed = Mathf.Abs (speed - target_speed) > accel * 0.03 && speed <= 4 && speed >= -1;
		}
	}

	/// <summary>
	/// Grenzt den speed-Wert ein
	/// </summary>
	public void clamp_speed(){
		speed = Mathf.Clamp (speed, -1, 4);
	}

	/// <summary>
	/// Startet die Mission, nachdem das Raumschiffreingewarpt ist
	/// </summary>
	void start_mission(){
		//warpspur.SetActive (false);
		if (auspuff!=null)
			auspuff.SetActive (true);
		Mission.current_mission.start_change_camera_focus ();

	}

	/// <summary>
	/// Initialisiert den Auto-Piloten
	/// </summary>
	void auto_navigate_start(){
		is_auto_navigating = true;

		auto_navigation_target_speed = 4;
	}
	/// <summary>
	/// Auto-Pilot zu einem GameObject
	/// </summary>
	/// <param name="point">Zielobjekt.</param>
	public void auto_navigate_to_object(GameObject point){
		auto_navigate_start();
		auto_navigation_target_object = point;
	}

	/// <summary>
	/// Auto-Pilot zu einem GameObject mit neuer target_speed
	/// </summary>
	/// <param name="point">Zielobjekt.</param>
	/// <param name="target_speed">Target speed.</param>
	public void auto_navigate_to_object(GameObject point, float target_speed){
		auto_navigate_to_object (point);
		auto_navigation_target_speed = target_speed;
	}
	/// <summary>
	/// Auto-Pilot zu einem Punkt
	/// </summary>
	/// <param name="point">Point.</param>
	public void auto_navigate_to_point(Vector3 point){
		auto_navigate_start ();
		auto_navigation_target_point = point == Vector3.zero ? new Vector3(0,0,0.1f) : point;
		auto_navigation_target_object = null;
	}
	/// <summary>
	/// Bricht den Auto-Piloten ab
	/// </summary>
	public void abort_auto_navigation(){
		is_auto_navigating = false;
	}

	/// <summary>
	/// Auto-Pilot update
	/// </summary>
	void auto_navigate(){
		if ((auto_navigation_target_object == null && auto_navigation_target_point == Vector3.zero) || warping_in) {
			abort_auto_navigation ();
		}

		Vector3 target_point = (auto_navigation_target_object == null) ? auto_navigation_target_point : auto_navigation_target_object.transform.position; // Zielpunkt des Auto-Piloten
		float dist = Vector3.Distance (transform.position, target_point); // verbleibende Entfernung

		if (dist<auto_navigation_target_distance) { // bricht ab, wenn die Zieldistanz erreicht ist
			abort_auto_navigation ();
			set_target_speed (0);
			return;
		}
			
		Vector3 dir_to_target = (target_point - transform.position).normalized; // Richtungsvektor zum Zielpunkt
		float angle = Vector3.Angle (transform.forward, dir_to_target); // Winkel zwischen Forwärtsvektor des Raumschiffs und Richtungsvektor zum Ziel
		if (angle <= min_navigation_angle) { // sollte der Winkel kleiner als min_navigation_angle sein, also das Raumschiff etwa auf das Ziel gucken, fliegt es einfach geradeaus
			set_target_speed (auto_navigation_target_speed);
			fix_z_rotation ();
			if (dist <= 200) {
				set_target_speed (2); // bei geringer Distanz zum Ziel wird die Geschwindigkeit gedrosselt
			}
		} else { // Wenn das Raumschiff nicht auf das Ziel guckt, muss es gedreht werden
			set_target_speed (Mathf.Min(auto_navigation_target_speed, 2.2f));
			Vector3 rot_axis = Vector3.Cross (transform.forward.normalized, dir_to_target); // die Drehachse ist dabei einfach ein Vektor der senkrecht auf Forwärtsvektor und Richtungsvektor zum Ziel steht, 
																							// also das Kreuzprodukt
			bool bug_180 = rot_axis.magnitude < 0.03f || Mathf.Abs(180-angle)<4; // falls der Winkel nahe an 180° ist wird noch eine weitere Achse (correction_axis) hinzugezogen
			rot_axis.Normalize ();
			if (bug_180) {
				Vector3 new_dir = Quaternion.Euler (auto_navigation_bug_180_correction_axis) * dir_to_target;
				Vector3 new_rot_axis = Vector3.Cross (transform.forward.normalized, new_dir.normalized);
				rot_axis = new_rot_axis;
			}
			transform.Rotate (rot_axis, rotation_speed * Time.deltaTime, Space.World);
		}
	}

	/// <summary>
	/// Setzt eine Zielgeschwindigkeit für das Raumschiff
	/// </summary>
	/// <param name="s">Zielgeschwindigkeit.</param>
	public void set_target_speed(float s) {
		has_target_speed = true;
		target_speed = s;
	}

	// shield colors ------------------------

	/*public void set_vertex_color_of_shield_quarter(GameObject shield, Color color){ // TODO: material instances statt vertex colors
		Mesh mesh = shield.GetComponent<MeshFilter>().mesh;
		int vert_number = mesh.vertices.Length;
		Color[] colors = new Color[vert_number];
		for (int i = 0; i < vert_number; i++) {
			colors [i] = color;
		}
		mesh.colors = colors;
	}*/

	/// <summary>
	/// Legt die Farbe für ein Viertelkreis-Schildobject fest
	/// </summary>
	/// <param name="shield">Schild-Object.</param>
	/// <param name="color">Color.</param>
	public void set_shieldpart_color(GameObject shield, Color color){
		shield.GetComponent<Renderer> ().material.color = color;
		shield.GetComponent<Renderer>().material.SetColor("_TintColor", color);
	//	shield.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", color);
	}

	/// <summary>
	/// Berechnet die Schildfarbe für die prozentuale Schildstärke p
	/// </summary>
	/// <returns>The shield color.</returns>
	/// <param name="p">prozentuale Schildstärke</param>
	public Color get_shield_color(float p){
		/*if (p > 0.5) {
			return Color.Lerp (shield_middle_color, shield_full_color, (p-0.5f)*2);
		} else if (p>0){
			return Color.Lerp (shield_down_color, shield_middle_color, p*2);
		}*/
		if (p > 0) {
			//return shield_color_gradient.Evaluate (1 - p);
			return Color.Lerp(shield_down_color, shield_full_color, p);
		}
		return new Color (1, 1, 1, 0);
	}

	/// <summary>
	/// Legt die Farben für die Schild-Viertelkreise fest
	/// </summary>
	public void set_shield_indicator_color(){
		float g = schild.max_shield_power_per_quarter;

		float top = schild.get_shield_part (SchildPartTypes.Top).shield_intensity / g;
		float bot = schild.get_shield_part (SchildPartTypes.Bot).shield_intensity / g;
		float left = schild.get_shield_part (SchildPartTypes.Left).shield_intensity / g;
		float right = schild.get_shield_part (SchildPartTypes.Right).shield_intensity / g;

		set_shieldpart_color (shield_top, get_shield_color (top));
		set_shieldpart_color (shield_bot, get_shield_color (bot));
		set_shieldpart_color (shield_left, get_shield_color (left));
		set_shieldpart_color (shield_right, get_shield_color (right));

	}

	/// <summary>
	/// Tarnt das Raumschiff
	/// </summary>
	public void cloak(){
		is_cloaking = true;

		StatusTexts.status_texts.new_text ("Getarnt");
		material.color = new Color (1, 1, 1, 1);
		Utils.SetupMaterialWithBlendMode (material, BlendMode.Fade); // Material BlendMode geändert, um Durchsichtig erscheinen zu können
		StopCoroutine ("cloak_animation");
		StartCoroutine ("cloak_animation");
	}
	/// <summary>
	/// Enttarnt das Raumschiff
	/// </summary>
	public void decloak(){
		CloakingDevice c = SpaceshipModule.get_module_of_type<CloakingDevice> (modules);

		// Nach dem Enttarnen erhält das Raumschiff für kurze Zeit einen Geschwindigkeits- und Schadensbuff
		apply_statuseffect (new StatusEffect { 
			name = "Speed Buff",
			description = "Geschwindigkeit ist erhöht", 
			duration = 1.5f, 
			effect_type = EffectTypes.Speed, 
			effect_intensity = c.speed_buff, 
			multiple_effects_intensity_multplier = 1.1f
		});

		apply_statuseffect (new StatusEffect { 
			name = "Damage Buff",
			description = "Schaden ist erhöht", 
			duration = 1.5f, 
			effect_type = EffectTypes.DamageOutput, 
			effect_intensity = c.damage_buff, 
			multiple_effects_intensity_multplier = 1
		});

		is_cloaking = false;
		StatusTexts.status_texts.new_text ("Enttarnt");
		StopCoroutine ("cloak_animation");
		StartCoroutine ("cloak_animation");
		//renderer.sharedMaterial = primary_material;
	}

	/// <summary>
	/// Animation für das Tarnen/Enttarnen des Raumschiffs
	/// </summary>
	IEnumerator cloak_animation(){
		while (true){
			if (is_cloaking) {
				material.color = new Color (1, 1, 1, material.color.a - Time.deltaTime * 0.5f);
				if (material.color.a < cloak_effect_alpha) {
					break;
				}
			} else {
				material.color = new Color (1, 1, 1, material.color.a + Time.deltaTime * 0.5f);
				if (material.color.a > cloak_effect_alpha) {
					material.color = new Color (1, 1, 1, 1);
					Utils.SetupMaterialWithBlendMode (material, BlendMode.Opaque);
					break;
				}
			}
			yield return null;
		}
	}

	// fight -----------------------------
	/// <summary>
	/// Richtet Schaden am Raumschiff an
	/// </summary>
	/// <param name="dmg_to_shield">Der Schaden, der dem Schild hinzugefügt wird.</param>
	/// <param name="schildpart">Der zu beschädigende Schildpart.</param>
	/// <param name="dmg_to_si">Der Schaden, der der Hülle hinzugefügt wird.</param>
	public void apply_damage(float dmg_to_shield, SchildPartTypes schildpart, float dmg_to_si){ // si= strukturelle integrität
		if (destroyed) return;


		float schild_int_before = schild.get_shield_intensity_at_part(schildpart)/schild.max_shield_power_per_quarter;
		schild.apply_dmg_at_shield_part(schildpart, dmg_to_shield);
		float schild_int_after = schild.get_shield_intensity_at_part (schildpart)/schild.max_shield_power_per_quarter;

		float si_before = this.structural_integrity;
		this.structural_integrity -= dmg_to_si;
		float si_after = this.structural_integrity;

		check_destroyed ();

		if (is_cloaking)
			decloak ();

		//check_out_of_combat ();
		if (this == Player.player.spaceship) { // falls dies das Spielerraumschiff oder das Raumschiff, auf das der Spieler gerade feuert, ist, 
											   // werden möglicherweise Nachrichten auf dem Bildschirm ausgegeben
			if (si_before > 0.5 && si_after <= 0.5 && si_after>0.25) {
				StatusTexts.status_texts.new_text ("Hüllenintegrität auf 50%");
			}
			else if (si_before > 0.25 && si_after <= 0.25 && si_after>0) {
				StatusTexts.status_texts.new_text ("Hüllenintegrität auf 25%");
			}
			else if (si_before > 0 && si_after == 0) {
				StatusTexts.status_texts.new_text ("Schiff zerstört");
			}
			if (schild_int_before > 0.5 && schild_int_after <= 0.5 && schild_int_after>0.25) {
				string seite = Schild.getPartName (schildpart);
				StatusTexts.status_texts.new_text (seite + " Schilde auf 50%");
			} else if (schild_int_before > 0 && schild_int_after == 0) {
				string seite = Schild.getPartName (schildpart);
				StatusTexts.status_texts.new_text (seite + " Schilde ausgefallen!");
			}
			if (!ship_under_attack) {
				AlarmUI.alarmUI.alarm ();
				StatusTexts.status_texts.new_text ("Schiff ist unter Beschuss");
			}
		} else if (this == PlayerScript.playerScript.get_enemy()) {
			float a = get_angle (PlayerScript.playerScript.transform.position);
			SchildPartTypes part = schild.get_schildpart_at_angle (a);
			if (part == schildpart) {
				if (schild_int_before > 0.5 && schild_int_after <= 0.5 && schild_int_after>0) {
					StatusTexts.status_texts.new_text ("Die uns zugewandten Schilde des feindlichen Schiffes sind auf 50%");
				} else if (schild_int_before > 0 && schild_int_after == 0 && si_after>0) {
					StatusTexts.status_texts.new_text ("Die uns zugewandten Schilde des feindlichen Schiffes sind ausgefallen");
				}
			}
		}
		last_time_dmg_received = Time.time;
	} 

	/// <summary>
	/// Prüft, ob das Raumschiff zerstört werden soll und tut dies ggf.
	/// </summary>
	public void check_destroyed(){
		if (destroyed)
			return;
		if (structural_integrity <= 0) {
			structural_integrity = 0;
			destroy ();
		}
	}

	/// <summary>
	/// Zerstört das Raumschiff
	/// </summary>
	public void destroy(){
		if (immortal)
			return;
		
		GameObject ex = GameObject.Instantiate (OtherPrefabObjects.otherPrefabObjects.ship_explosion); // Schiffexplosion effect
		ex.transform.position = transform.position;

		GameObject.Destroy (ex, 3);


		destroyed = true;

		// Auf alle Meshes die zu dem Raumschiff gehören wird das DeformMesh-Script gelegt, um das Raumschiff kaputt aussehen zu lassen
		MeshFilter[] filters = gfx_parent == gameObject ? new MeshFilter[]{ GetComponent<MeshFilter> () } : gfx_parent.GetComponentsInChildren<MeshFilter> ();

		foreach (MeshFilter r in filters) {
			DeformMesh dm = r.gameObject.AddComponent<DeformMesh> ();
			dm.deform_factor = destroyed_deform_factor;

			dm.deform ();
		}

		schild_collision_object.SetActive (false); // schild objekte werden ausgeschaltet
		shields.SetActive (false);

		//da das Raumschiff zerstört ist, muss es auch nicht mehr auf der Minimap angezeigt werden
		Minimap.minimap.check_objects ();

		// Collision mit diesem Object wird disabled
		Collider[] cols = GetComponentsInChildren<Collider> ();
		foreach (Collider col in cols) {
			col.enabled = false;
		}


		if (rigidbody.velocity.magnitude == 0) {
			rigidbody.AddForce (Utils.random_vector3 (1) * 10000);

		}
		GetComponent<Rigidbody> ().drag = 0.9f;

		// wenn dies das Spieler-Raumschiff ist, wird der SpielerInput (PlayerScript) disabled und der DeathTimer aktiviert
		if (this == Player.player.spaceship) {
			// reset at spawnpoint

			UI.ui.death_timer.SetActive (true);
			PlayerScript.playerScript.enabled = false;
			this.enabled = false;
			return;
		}

		// wenn dies nicht das Spielerraumschiff ist, wird ein zufälliges Item gedroppt
		List<Item> items = equipped_items;
		if (spaceship_group_type == ObjectGroupType.Enemy)
			Drop.drop_item (transform.position, items[Random.Range(0, items.Count-1)]);

		// Wenn dies das Schiff ist, das der Spieler angegriffen hat, wird eine Meldung auf dem Bildschirm angezeigt
		if (this == Spaceship.get_spaceship(PlayerScript.playerScript.selected_enemy)) {
			PlayerScript.playerScript.selected_enemy = null;
			StatusTexts.status_texts.new_text ("Feindliches Schiff zerstört");
		}

		// Wenn dies ein Ziel der Mission war, ist dieses jetzt erfüllt
		foreach (MissionGoal g in Mission.current_mission.fullMission.get_current_mission_part().mission_goals) {
			if (g.mission_goal_type == MissionGoalTypes.DestroySpaceship && g.target == gameObject) {
				g.goal_achieved = true;
			}
		}

		// Mission update
		Mission.current_mission.check_status ();

		this.enabled = false;
	}
		
	/// <summary>
	/// Prüft, ob dieses <see cref="Spaceship"/> angreifen kann.
	/// </summary>
	/// <value><c>true</c> wenn das Raumschiff angreifen kann; sonst <c>false</c>.</value>
	public bool can_attack{
		get{
			return !StatusEffect.is_stunned (status_effects);
		}
	}
	/// <summary>
	/// Gibt an, ob das <see cref="Spaceship"/> unter Beschuss ist
	/// </summary>
	/// <value><c>true</c> wenn ja; sonst, <c>false</c>.</value>
	public bool ship_under_attack{
		get{ 
			return (Time.time - last_time_dmg_received <= Spaceship.min_out_of_combat_time);
		}
	}
	/// <summary>
	/// Gibt an, ob das <see cref="Spaceship"/> im Kampf ist.
	/// </summary>
	/// <value><c>true</c> wenn ja; sonst, <c>false</c>.</value>
	public bool in_battle{
		get{ 
			return (Time.time - last_time_attack <= Spaceship.min_out_of_combat_time) || ship_under_attack;
		}
	}
	/// <summary>
	/// Gibt den Winkel der der Punkt global_pos auf der Raumschiff-Ebene mit dem Vorwärtsvektor bildet
	/// 0° bedeutet z.B. Vorne
	/// </summary>
	/// <returns>Der Winkel.</returns>
	/// <param name="global_pos">Globale Position.</param>
	public float get_angle(Vector3 global_pos){
		Vector3 loc_p = transform.InverseTransformPoint (global_pos);
		loc_p.y = 0;
		if (loc_p.magnitude == 0) {
			return 0;
		}
			
		float angle = Vector3.Angle(Vector3.forward, loc_p);
		if (loc_p.x < 0) {
			angle *= -1;
		}
		return angle;
	}

	/// <summary>
	/// Feuert einen weiteren Phaser einer Phaser-Salve ab
	/// </summary>
	/// <param name="weapon">Waffentyp.</param>
	/// <param name="p_enemy">Gegner.</param>
	/// <param name="layer">Layer mask.</param>
	/// <param name="effect">Phaser-Salve-Effect.</param>
	void fire_subsequent_phaser(Weapon weapon, GameObject p_enemy, int layer, WeaponEffect effect){
		Phaser new_phaser = ((Phaser)weapon).get_copy ();
		new_phaser.base_damage = weapon.base_damage * effect.subsequent_weapon_damage_multiplier;
		new_phaser.start_pos = weapon.weapon_position;//.transform.position;
		GameObject temp_endpoint = new GameObject ("temp_phaser_endpoint");
		Destroy (temp_endpoint, 2);
		temp_endpoint.transform.position = p_enemy.transform.position + new Vector3 (Random.Range (-phaser_fail_tolerance, phaser_fail_tolerance),
			Random.Range (-phaser_fail_tolerance, phaser_fail_tolerance), Random.Range (-phaser_fail_tolerance, phaser_fail_tolerance));
		new_phaser.end_pos = temp_endpoint;
		new_phaser.deal_damage (layer);	

		new_phaser.shoot ();
	}

	/// <summary>
	/// Greift den Gegner an
	/// </summary>
	/// <param name="enemy">Enemy.</param>
	/// <param name="layer">Layer.</param>
	/// <param name="weapon_type">Weapon type.</param>
	public void attack_enemy (GameObject enemy, int layer, WeaponRawTypes weapon_type){
		attack_enemy (enemy, layer, weapon_type, true);
	}
		
	/// <summary>
	/// Greift den Gegner an
	/// </summary>
	/// <param name="enemy">Enemy.</param>
	/// <param name="layer">Layer.</param>
	/// <param name="weapon_type">Weapon type.</param>
	/// <param name="recursive">Gibt an, ob die selbe Waffe auf ein anderes Schiff feuern kann, wennn das ausgewählte Schiff nicht durch diese Waffe angegriffen werden kann. (Zurzeit nicht aktiv)</param>
	public void attack_enemy(GameObject enemy, int layer, WeaponRawTypes weapon_type, bool recursive){
		if (this.destroyed)
			return;
		if (!this.can_attack)
			return;

		/*Spaceship s = Spaceship.get_spaceship (enemy);

		if (s.destroyed)
			return;*/

		float dist_to_enemy = Vector3.Distance (transform.position, enemy.transform.position);
		if (dist_to_enemy > Spaceship.max_attack_distance) {
			return;
		}

		List<Spaceship> all_enemies = Spaceship.get_active_enemy_spaceships ();

		//List<Weapon> wl = get_possible_weapons (enemy);
		//foreach (Weapon weapon in wl) {
		foreach (SpaceshipWeaponPosition spaceshipWeaponPosition in spaceshipWeaponPositions) { // Schleife durch alle Waffenpositionen
			Vector3 enemy_to_pos_dir = enemy.transform.position - spaceshipWeaponPosition.transform.position;
			float angle = Vector3.Angle (enemy_to_pos_dir, transform.TransformDirection (spaceshipWeaponPosition.local_direction)); // Winkel zwischen Waffenpositionsrichtung und Richtungsvektor zum Gegner
			foreach (Weapon weapon in spaceshipWeaponPosition.weapons) { // Schleife durch alle Waffen dieser Waffenposition
				if (angle < weapon.arc_range / 2 && (enemy_to_pos_dir.magnitude<Spaceship.max_attack_distance)) { // hat diese Waffe einen ausreichenden Winkel zu dem Gegner
					if (weapon.can_shoot ()) { // Kann die Waffe schießen oder hat sie Cooldown
						if (weapon.GetType ().Name == "Phaser" && (weapon_type & WeaponRawTypes.Phaser) == WeaponRawTypes.Phaser) { // Ist die Waffe vom Typ Phaser?

							last_time_attack = Time.time;
							if (is_cloaking) // durch das Angreifen wird die Tarnvorrichtung deaktiviert
								decloak ();
							weapon.damage_output_multiplier = damage_output;

							//List<GameObject> enemies = new List<GameObject> ();
							if (Player.player.spaceship == this){ // schießt der spieler?
								WeaponEffect mult_phaser_targets = has_weapon_effect (WeaponEffectTypes.MultiplePhaserTargets); // ist ein multiplePhaserTargets buff aktiv?
								if (mult_phaser_targets!=null){
									if (!mult_phaser_targets.enabled) {
										mult_phaser_targets.enabled = true;
										mult_phaser_targets.start_time = Time.time;
										mult_phaser_targets.objects_fired = 0;
										Invoke ("check_status_effects", mult_phaser_targets.duration);
									}

									foreach (Spaceship p_enemy in all_enemies) { // dann auf alle gegner die für dieser Phaser treffen könnte feuern
										if (p_enemy.gameObject == enemy)
											continue;
										if (mult_phaser_targets.objects_fired >= mult_phaser_targets.max_extra_objects && mult_phaser_targets.max_extra_objects != -1)
											break;
										
										float p_angle = Vector3.Angle (p_enemy.transform.position - spaceshipWeaponPosition.transform.position, transform.TransformDirection (spaceshipWeaponPosition.local_direction));
										if (p_angle < weapon.arc_range / 2) {
											fire_subsequent_phaser (weapon, p_enemy.gameObject, layer, mult_phaser_targets);
											mult_phaser_targets.objects_fired += 1;
										}
									}
								}

								WeaponEffect phaser_salve = has_weapon_effect (WeaponEffectTypes.PhaserSalve); // ist ein PhaserSalve Buff aktiv
								if (phaser_salve != null) {
									if (!phaser_salve.enabled) {
										phaser_salve.enabled = true;
										phaser_salve.start_time = Time.time;
										phaser_salve.objects_fired = 0;
										Invoke ("check_status_effects", phaser_salve.duration);
									}
									// Coroutine mit den weiteren Phasern starten
									WaitForSeconds wfs = new WaitForSeconds (0.3f);
									phaser_salve_coroutine = phaser_salve_function (wfs, phaser_salve.max_extra_objects == -1 ? (int)((phaser_salve.start_time+phaser_salve.duration-Time.time)/0.3f) : phaser_salve.max_extra_objects, weapon, enemy, layer, phaser_salve);
									StartCoroutine (phaser_salve_coroutine);
								}

							}

							//normalen Phaser schießen

							((Phaser)weapon).start_pos = weapon.weapon_position;//.transform.position;
							GameObject temp = new GameObject ("temp_phaser_endpoint");
							Destroy (temp, 2);
							temp.transform.position = enemy.transform.position + new Vector3 (Random.Range (-phaser_fail_tolerance, phaser_fail_tolerance),
								Random.Range (-phaser_fail_tolerance, phaser_fail_tolerance), Random.Range (-phaser_fail_tolerance, phaser_fail_tolerance));
							((Phaser)weapon).end_pos = temp;
							((Phaser)weapon).deal_damage (layer);	

							weapon.shoot ();


						} else if (weapon.GetType ().Name == "Torpedo" && (weapon_type & WeaponRawTypes.Torpedo) == WeaponRawTypes.Torpedo) { // ist die Waffe ein Torpedo?

							last_time_attack = Time.time;
							if (is_cloaking)
								decloak ();
							weapon.damage_output_multiplier = damage_output;

							((Torpedo)weapon).target_object = enemy;
							((Torpedo)weapon).enemy_layer = layer; // layer festlegen
							weapon.shoot ();

						} else if (weapon.GetType ().Name == "PulsPhaser" && (weapon_type & WeaponRawTypes.PulsPhaser) == WeaponRawTypes.PulsPhaser) { // ist die Waffe ein PulsPhaser?

							last_time_attack = Time.time;
							if (is_cloaking)
								decloak ();
							weapon.damage_output_multiplier = damage_output;

							((PulsPhaser)weapon).target_object = enemy;
							((PulsPhaser)weapon).enemy_layer = layer;
							weapon.shoot ();
						}
					}
				} else if (recursive && Player.player.spaceship == this && false){
					foreach (Spaceship s in Spaceship.get_active_enemy_spaceships()) {
						attack_enemy (s.gameObject, layer, weapon_type, false);
					}
				}
			}
		}
		
	}
	/// <summary>
	/// Zeichnet den Angriffsradius um das Raumschiff
	/// </summary>
	void OnDrawGizmosSelected(){
		Gizmos.DrawWireSphere (transform.position, max_attack_distance);
	}
}
