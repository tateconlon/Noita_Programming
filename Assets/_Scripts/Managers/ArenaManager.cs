using System;
using System.Collections;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ArenaManager : MonoBehaviour
{
    [SerializeField] RuntimeSetGameObject enemies;
    [SerializeField] SpawnManager spawnManager;

    public UnityEvent onBeginCountdown;

    public UnityEvent<int> onLevelWon;

    int _level = 1;  // TODO: make this start at 0. Will require checking lots of off-by-one cases
    int _loop = 0;

    public ArenaState ArenaState { get; private set; } = ArenaState.None;
    
    WinCondition _winCondition = WinCondition.None;  // Replaces boss_level and win_condition in source

    [FormerlySerializedAs("wave")] public GlobalInt waveCur;
    public GlobalInt waveMax;
    
    GameObject _boss;
    
    public void EnterArena()
    {
        // TODO: temp/unused
        {
            // _currstate = ArenaState.PreFight;
            // M.am.PlaySfx(SfxType.None);
        }

        // Cursor.visible = M.im.useMouseControl;
        
        // trigger:tween(2, main_song_instance, {volume = 0.5, pitch = 1}, math.linear)
        //
        // steam.friends.setRichPresence('steam_display', '#StatusFull')
        // steam.friends.setRichPresence('text', 'Arena - Level ' .. self.level)
        //
        // self.t:every(0.375, function()
        //   local p = random:table(star_positions)
        //   Star{group = star_group, x = p.x, y = p.y}
        // end)
        
        SetupVariables();
        
        SpawnSnake();
        
        spawnManager.Initialize(ShouldSpawnEnemies, CheckWinCondition);

        if (_level == 1)
        {
            ShowTutorial();
        }

        if (_level == 1000)
        {
            SetupLevel1000();
        }
        else if ((_level - (25 * _loop)) % 6 == 0 || _level % 25 == 0)
        {
            _winCondition = WinCondition.Boss;
        }
        else
        {
            _winCondition = WinCondition.Waves;
        }
        
        StartCoroutine(spawnManager.RetryPreventedEnemySpawns());
        
        // TODO: add 1 sec delay here to match source code
        
        onBeginCountdown.Invoke();
    }

    /// <remarks>
    /// Source code arena.lua lines 11-21, 61-80
    /// </remarks>
    void SetupVariables()
    {
        // self.hfx:add('condition1', 1)
        // self.hfx:add('condition2', 1)
        // self.level = level or 1
        // self.loop = loop or 0
        // self.units = units
        // self.passives = passives
        // self.shop_level = shop_level or 1
        // self.shop_xp = shop_xp or 0
        // self.lock = lock
        //
        // self.starting_units = table.copy(units)
        //
        // self.gold_picked_up = 0
        // self.damage_dealt = 0
        // self.damage_taken = 0
        // self.main_slow_amount = 1
        // self.enemies = {Seeker, EnemyCritter}
        // self.color = self.color or fg[0]

        ArenaState = ArenaState.PreFight;
    }

    /// <remarks>
    /// Source code arena.lua lines 91-103, 260-280
    /// </remarks>
    void SpawnSnake()
    {
        // for i, unit in ipairs(units) do
        //   if i == 1 then
        //     self.player = Player{group = self.main, x = gw/2, y = gh/2 + 16, leader = true, character = unit.character, level = unit.level, passives = self.passives, ii = i}
        //   else
        //     self.player:add_follower(Player{group = self.main, character = unit.character, level = unit.level, passives = self.passives, ii = i})
        //   end
        // end
        //
        // local units = self.player:get_all_units()
        // for _, unit in ipairs(units) do
        //   local chp = CharacterHP{group = self.effects, x = self.x1 + 8 + (unit.ii-1)*22, y = self.y2 + 14, parent = unit}
        //   unit.character_hp = chp
        // end
        
        
        // -- Calculate class levels
        // local units = {}
        // table.insert(units, self.player)
        // for _, f in ipairs(self.player.followers) do table.insert(units, f) end
        //
        // local class_levels = get_class_levels(units)
        // self.ranger_level = class_levels.ranger
        // self.warrior_level = class_levels.warrior
        // self.mage_level = class_levels.mage
        // self.rogue_level = class_levels.rogue
        // self.nuker_level = class_levels.nuker
        // self.curser_level = class_levels.curser
        // self.forcer_level = class_levels.forcer
        // self.swarmer_level = class_levels.swarmer
        // self.voider_level = class_levels.voider
        // self.enchanter_level = class_levels.enchanter
        // self.healer_level = class_levels.healer
        // self.psyker_level = class_levels.psyker
        // self.conjurer_level = class_levels.conjurer
        // self.sorcerer_level = class_levels.sorcerer
        // self.mercenary_level = class_levels.mercenary
    }

    /// <remarks>
    /// Source code arena.lua lines 251-258
    /// </remarks>
    void ShowTutorial()
    {
        // local t1 = Text2{group = self.floor, x = gw/2, y = gh/2 + 2, sx = 0.6, sy = 0.6, lines = {{text = '[light_bg]<- or a         -> or d', font = fat_font, alignment = 'center'}}}
        // local t2 = Text2{group = self.floor, x = gw/2, y = gh/2 + 18, lines = {{text = '[light_bg]turn left                                      turn right', font = pixul_font, alignment = 'center'}}}
        // local t3 = Text2{group = self.floor, x = gw/2, y = gh/2 + 46, sx = 0.6, sy = 0.6, lines = {{text = '[light_bg]esc - options', font = fat_font, alignment = 'center'}}}
        // t1.t:after(8, function() t1.t:tween(0.2, t1, {sy = 0}, math.linear, function() t1.sy = 0 end) end)
        // t2.t:after(8, function() t2.t:tween(0.2, t2, {sy = 0}, math.linear, function() t2.sy = 0 end) end)
        // t3.t:after(8, function() t3.t:tween(0.2, t3, {sy = 0}, math.linear, function() t3.sy = 0 end) end)
    }

    /// <remarks>
    /// Source code arena.lua lines 105-106
    /// </remarks>
    void SetupLevel1000()
    {
        // self.level_1000_text = Text2{group = self.ui, x = gw/2, y = gh/2, lines = {{text = '[fg, wavy_mid]SNKRX', font = fat_font, alignment = 'center'}}}
    }

    public void OnCountdownComplete()
    {
        switch (_winCondition)
        {
            case WinCondition.Boss:
                StartCoroutine(SetupLevelBoss());
                break;
            case WinCondition.Waves:
                StartCoroutine(SetupLevelWaves());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <remarks>
    /// Source code arena.lua lines 108-177
    /// </remarks>
    IEnumerator SetupLevelBoss()
    {
        Debug.Log($"Starting boss for level {_level}");
        ArenaState = ArenaState.Fight;
        
        // alert1:play{pitch = 1.2, volume = 0.5}
        // camera:shake(4, 0.25)
        // SpawnEffect{group = self.effects, x = gw/2, y = gh/2 - 48}

        // Spawn the boss
        yield return spawnManager.SpawnBoss(_level, enemy => _boss = enemy);
        
        // Continuously spawn other enemies throughout the level
        while (ArenaState == ArenaState.Fight)
        {
            while (enemies.elementCount > (_boss != null && _boss.activeInHierarchy ? 1 : 0) ||
                   spawnManager.isSpawningEnemies)
            {
                yield return null;
            }
            
            yield return spawnManager.SpawnEnemiesBossLevel(_level);
        }
    }

    /// <remarks>
    /// Source code arena.lua lines 178-249
    /// </remarks>
    IEnumerator SetupLevelWaves()
    {
        waveCur.value = 0;
        waveMax.value = D.gv.levelToMaxWaves[(_level - 1) % D.gv.levelToMaxWaves.Count];
        
        Debug.Log($"Starting waves for level {_level}");
        ArenaState = ArenaState.Fight;
        
        // alert1:play{pitch = 1.2, volume = 0.5}
        // camera:shake(4, 0.25)
        // SpawnEffect{group = self.effects, x = gw/2, y = gh/2 - 48}

        while (waveCur < waveMax)
        {
            while (enemies.elementCount > 0 || spawnManager.isSpawningEnemies)
            {
                yield return null;
            }
            
            yield return spawnManager.SpawnEnemiesWaveLevel(_level, waveCur, _loop);
            
            waveCur.value += 1;
        }
        
        Debug.Log("Reached end of waves");
        
        CheckWinCondition();
        
        //   if self.level == 20 and self.trailer then
        //     Text2{group = self.ui, x = gw/2, y = gh/2 - 24, lines = {{text = '[fg, wavy]SNKRX', font = fat_font, alignment = 'center'}}}
        //     Text2{group = self.ui, x = gw/2, y = gh/2, sx = 0.5, sy = 0.5, lines = {{text = '[fg, wavy_mid]play now!', font = fat_font, alignment = 'center'}}}
        //     Text2{group = self.ui, x = gw/2, y = gh/2 + 24, sx = 0.5, sy = 0.5, lines = {{text = '[light_bg, wavy_mid]music: kubbi - ember', font = fat_font, alignment = 'center'}}}
        //   end
    }

    
    public void OnEnemyDisabled()
    {
        CheckWinCondition();
    }

    bool ShouldSpawnEnemies()
    {
        return ArenaState == ArenaState.Fight;
    }

    void CheckWinCondition()
    {
        if (ArenaState != ArenaState.Fight) return;
        if (spawnManager.isSpawningEnemies) return;
        
        switch (_winCondition)
        {
            case WinCondition.None:
                break;
            case WinCondition.Boss:
                if (enemies.elementCount == 0 && (_boss == null || !_boss.activeInHierarchy))
                {
                    WinLevel();
                }
                break;
            case WinCondition.Waves:
                if (enemies.elementCount == 0 && waveCur >= waveMax)
                {
                    WinLevel();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <remarks>
    /// Source code arena.lua quit()
    /// </remarks>
    public void WinLevel()
    {
        if (ArenaState != ArenaState.Fight) return;
        ArenaState = ArenaState.PostFight;
        
        onLevelWon.Invoke(_level);
        // TODO: Things can listen to this, then read from game state SO to know how to act (e.g. AchievementSystem)
        // (Also add ability to convert from game state SO to save file and vice-versa?)
        
        // TODO: boss level achievements
        // if self.level == 6 then
        //   state.achievement_speed_booster = true
        //   system.save_state()
        //   steam.userStats.setAchievement('SPEED_BOOSTER')
        //   steam.userStats.storeStats()
        // elseif self.level == 12 then
        //   state.achievement_exploder = true
        //   system.save_state()
        //   steam.userStats.setAchievement('EXPLODER')
        //   steam.userStats.storeStats()
        // elseif self.level == 18 then
        //   state.achievement_swarmer = true
        //   system.save_state()
        //   steam.userStats.setAchievement('SWARMER')
        //   steam.userStats.storeStats()
        // elseif self.level == 24 then
        //   state.achievement_forcer = true
        //   system.save_state()
        //   steam.userStats.setAchievement('FORCER')
        //   steam.userStats.storeStats()
        // elseif self.level == 25 then
        //   state.achievement_cluster = true
        //   system.save_state()
        //   steam.userStats.setAchievement('CLUSTER')
        //   steam.userStats.storeStats()
        // end
    }
    
    /// <remarks>
    /// Source code arena.lua die()
    /// </remarks>
    void LoseLevel()
    {
        if (ArenaState != ArenaState.Fight) return;
        ArenaState = ArenaState.PostFight;
        
        // TODO
    }

    public void ExitArena()
    {
        //TODO: Stop Snake movement
        ArenaState = ArenaState.None;
    }

    enum WinCondition
    {
        None,
        Boss,
        Waves,
    }
}

public enum ArenaState
{
    None,
    PreFight,
    Fight,
    PostFight,
} 