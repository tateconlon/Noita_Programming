using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInit : MonoBehaviour
{
    public void Init(Transform parent, Vector3 pos, Quaternion rot, float velocity, Color color,
        float dmg, bool didCrit, string character, int level,
        //mods are below
        int chain, int pierce, int ricochet, float knockback, bool homing, 
        int spawnCrittersOnHit, int spawnCrittersOnKill)
    {
        // self.hfx:add('hit', 1)
        // self:set_as_rectangle(10, 4, 'dynamic', 'projectile')
        // self.pierce = args.pierce or 0
        // self.chain = args.chain or 0
        // self.ricochet = args.ricochet or 0
        // self.chain_enemies_hit = {}
        // self.infused_enemies_hit = {}
        
        // if self.parent.divine_machine_arrow and table.any(self.parent.classes, function(v) return v == 'ranger' end) then
        // if random:bool((self.parent.divine_machine_arrow == 1 and 10) or (self.parent.divine_machine_arrow == 2 and 20) or (self.parent.divine_machine_arrow == 3 and 30)) then
        // self.homing = true
        // self.pierce = self.parent.divine_machine_arrow or 0
        // end
        //     end
        //
        // if self.homing then
            // self.homing = false
            // self.t:after(0.1, function()
                // self.homing = true
                // self.closest_sensor = Circle(self.x, self.y, 64)
                // end)
        // end
        
        // self.distance_travelled = 0
        // self.distance_dmg_m = 1
        
        // if self.parent.blunt_arrow and table.any(self.parent.classes, function(v) return v == 'ranger' end) then
        // if random:bool((self.parent.blunt_arrow == 1 and 10) or (self.parent.blunt_arrow == 2 and 20) or (self.parent.blunt_arrow == 3 and 30)) then
        // self.knockback = 10
        // end
        //     end
        
        // if self.parent.flying_daggers and table.any(self.parent.classes, function(v) return v == 'rogue' end) then
        // self.chain = self.chain + ((self.parent.flying_daggers == 1 and 2) or (self.parent.flying_daggers == 2 and 3) or (self.parent.flying_daggers == 3 and 4))
        // end
    }

    void Update()
    {
        //Movement Code:
        // if self.homing then
            // self.closest_sensor:move_to(self.x, self.y)
            // local target = self:get_closest_object_in_shape(self.closest_sensor, main.current.enemies)
            // if target then
                // self:rotate_towards_object(target, 0.1)
                // self.r = self:get_angle()
                // self:move_along_angle(self.v, self.r + (self.orbit_r or 0))
            // else
                // self:set_angle(self.r)
                // self:move_along_angle(self.v, self.r + (self.orbit_r or 0))
                // end
        // else
            // self:set_angle(self.r)
            // self:move_along_angle(self.v, self.r + (self.orbit_r or 0))
        // end

    }

    void OnTriggerEnter(Collider col)
    {
        //if table.any(main.current.enemies, function(v) return other:is(v) end) then
            // if self.pierce <= 0 and self.chain <= 0 then
            //   self:die(self.x, self.y, nil, random:int(2, 3))
            // else
            //   --Deal with piercing
            //   if self.pierce > 0 then
            //     self.pierce = self.pierce - 1
            //   end
                //Deal with chaining
            //   if self.chain > 0 then
            //     self.chain = self.chain - 1
            //     table.insert(self.chain_enemies_hit, other)
            //     local object = self:get_random_object_in_shape(Circle(self.x, self.y, 48), main.current.enemies, self.chain_enemies_hit)
            //     if object then
            //       self.r = self:angle_to_object(object)
            //       if self.character == 'lich' then
            //         self.v = self.v*1.1
            //         if self.level == 3 then
            //           object:slow(0.2, 2)
            //         end
            //       else
            //         self.v = self.v*1.25
            //       end
            //       if self.level == 3 and self.character == 'scout' then
            //         self.dmg = self.dmg*1.25
            //       end
            //       if self.parent.ultimatum then
            //         self.dmg = self.dmg*((self.parent.ultimatum == 1 and 1.1) or (self.parent.ultimatum == 2 and 1.2) or (self.parent.ultimatum == 3 and 1.3))
            //       end
            //     end
            //   end
            //   HitCircle{group = main.current.effects, x = self.x, y = self.y, rs = 6, color = fg[0], duration = 0.1}
            //   HitParticle{group = main.current.effects, x = self.x, y = self.y, color = self.color}
            //   HitParticle{group = main.current.effects, x = self.x, y = self.y, color = other.color}
            // end

        //play sounds
        
        //DO DAMAGE
        //other:hit(self.dmg*(self.distance_dmg_m or 1), self)
        
        //if self.crit then
            // camera:shake(5, 0.25)
            // rogue_crit1:play{pitch = random:float(0.95, 1.05), volume = 0.5}
            // rogue_crit2:play{pitch = random:float(0.95, 1.05), volume = 0.15}
            // for i = 1, 3 do HitParticle{group = main.current.effects, x = other.x, y = other.y, color = self.color, v = random:float(100, 400)} end
            // for i = 1, 3 do HitParticle{group = main.current.effects, x = other.x, y = other.y, color = other.color, v = random:float(100, 400)} end
            // HitCircle{group = main.current.effects, x = other.x, y = other.y, rs = 12, color = fg[0], duration = 0.3}:scale_down():change_color(0.5, self.color)
        // end
        //
        // if self.knockback then
        //      other:push(self.knockback*(self.knockback_m or 1), self.r)
        // end
    }
}
