﻿using CombatEffectElements;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatPanel : UIPanel
{
	#region Variables
	public Camera mainCam;
	private CombatManager combatManager;

	[Header("Skills")]
	public Image PassImage;
	public Image RepositioningImage;
	public Image[] TeamSkillImage;
	public Image[] CooldownCoverImages;
	public TextMeshProUGUI[] Cooldowns;

	[Header("Entity Inspection Window")]
	public TextMeshProUGUI SkillDescriptionText;
	public TextMeshProUGUI StatDescriptionText;

	public Slider XpSlider;
	public Image EntityInspectionPortrait;
	public CombatEffectPool EntityInspectionEffectPool;
	public TextMeshProUGUI EntityNameText;
	public TextMeshProUGUI EntityLevelText;

	[Header("Team Portraits")]
	[SerializeField] private Image[] playerTeamPortraits;
	[SerializeField] private Image[] opposingTeamPortraits;
	private Image[,] teamPortraits;
	public Image[,] TeamPortraits
	{
		get
		{
			if (teamPortraits != null) return teamPortraits;

			teamPortraits = new Image[2,
				(playerTeamPortraits.Length > opposingTeamPortraits.Length)
				? playerTeamPortraits.Length
				: opposingTeamPortraits.Length];
			for (int x = 0; x < teamPortraits.GetLength(0); x++)
				for (int y = 0; y < teamPortraits.GetLength(1); y++)
					teamPortraits[x, y] = (x == 0) ? playerTeamPortraits[y] : opposingTeamPortraits[y];

			return teamPortraits;
		}
	}

	[Header("Character Positions")]
	[SerializeField] private GameObject[] playerCharacterPositions;
	[SerializeField] private GameObject[] opposingTeamCharacterPositions;
	private GameObject[,] characterPositions;
	public GameObject[,] CharacterPositions
	{
		get
		{
			if (characterPositions != null) return characterPositions;

			characterPositions = new GameObject[2,
				(playerCharacterPositions.Length > opposingTeamCharacterPositions.Length)
				? playerCharacterPositions.Length
				: opposingTeamCharacterPositions.Length];
			for (int x = 0; x < characterPositions.GetLength(0); x++)
				for (int y = 0; y < characterPositions.GetLength(1); y++)
					characterPositions[x, y] = (x == 0) ? playerCharacterPositions[y] : opposingTeamCharacterPositions[y];
			return characterPositions;
		}
	}

	private bool combatActive;
	public bool CombatActive
	{
		get { return combatActive; }
		set
		{
			combatActive = value;
			ToggleVisibility(value);
		}
	}

	private EventSystem eventSystem;
	public new EventSystem EventSystem
	{
		get { return eventSystem ?? (eventSystem = GetComponentInChildren<EventSystem>()); }
	}
	#endregion

	protected override void Awake()
	{
		base.Awake();
		transform.parent.GetComponentInChildren<EventSystem>().gameObject.SetActive(false);
		combatManager = AssetManager.Instance.GetManager<CombatManager>() ?? new CombatManager();
		combatManager.RegisterCombatPanel(this);
		CombatActive = false;
	}

	private void Start()
	{
		AssetManager instance = AssetManager.Instance;
		Entity[] ownTeam = instance.Savestate.CurrentTeam;
		Entity[] enemyTeam = instance.GetManager<DungeonManager>().BufferedEnemies;
		combatManager.StartCombat(ownTeam, enemyTeam);
	}

	private void OnApplicationQuit()
	{
		combatManager.PurgeAllCombatEffects();
		AssetManager.Instance.GetManager<GameManager>().OnApplicationQuit();
	}

	private void Update()
	{
		if(CombatActive) combatManager.UpdateCombatManager();
	}

	public void OnSkillSelect(int skillID)
	{
		if (CombatActive) combatManager.OnSkillSelect(skillID);
	}
}