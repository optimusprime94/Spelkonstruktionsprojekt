﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using ZEngine.Components;
using static Spelkonstruktionsprojekt.ZEngine.Wrappers.CollisionShape;

namespace Spelkonstruktionsprojekt.ZEngine.Helpers
{
    public interface IEntityBuilder
    {
        /// <summary>
        /// Can be used for when the user want's to add a component
        /// outside of the builder when using a if condition or for some
        /// other reason.
        /// </summary>
        /// <returns> entity id </returns>
        uint GetEntityKey();

        /// <summary>
        /// Gives the entity a vector to position.
        /// </summary>
        /// <param name="position"> entity coordinates </param>
        /// <param name="layerDepth"></param>
        /// <returns></returns>
        EntityBuilder SetPosition(Vector2 position, int layerDepth = 400);

        /// <summary>
        /// The sprite component can be added to the entity so that
        /// it can be rendered on the game screen.
        /// </summary>
        /// <param name="spriteName"> the name of the texture file </param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="scale"> texture minScale </param>
        /// <param name="alpha"> alpha value between 0-1, 0 will make it transparent </param>
        /// <returns></returns>
        EntityBuilder SetSprite(string spriteName, Point startPosition, int tileWidth, int tileHeight, float scale = 1,
            float alpha = 1, Texture2D sprite = null);

        /// <summary>
        /// Build is the last call to make, when all other component method
        /// calls have been made. Build will take all the components that have been
        /// created and add them to the entity in component manager.
        /// </summary>
        /// <returns></returns>
        EntityBuilder Build();

        /// <summary>
        /// This component is added if the entity is intended
        /// to behave as an AI.
        /// </summary>
        /// <param name="chasingDistance"> distance for when the AI will start follow players </param>
        /// <param name="isWandering"> if the AI is wandering or standing still </param>
        /// <returns></returns>
        EntityBuilder SetArtificialIntelligence(float chasingDistance = 250);

        /// <summary>
        /// This call will add the collision component, this will make the entity
        /// collidable with other game entities that have this component.
        /// </summary>
        /// <param name="boundingRectangle"></param>
        /// <param name="isCage"></param>
        /// <returns></returns>
        EntityBuilder SetCollision(bool isCage = false, VolumeType volumeType = VolumeType.Rectangle);

        /// <summary>
        /// A special case method for the user who decides to
        /// create the components outside of the builder. This
        /// will add the elements from the parameter.
        /// </summary>
        /// <param name="alreadyCreatedComponents"></param>
        /// <returns></returns>
        EntityBuilder SetCreatedComponents(List<IComponent> alreadyCreatedComponents);

        /// <summary>
        /// Adds dimensions to the entity.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        EntityBuilder SetDimensions(int width, int height);

        /// <summary>
        /// All entities that have the health component can
        /// be killed in the game.
        /// </summary>
        /// <param name="maxhealth"></param>
        /// <param name="currentHealth"></param>
        /// <param name="alive"> used as a flag</param>
        /// <returns></returns>
        EntityBuilder SetHealth(int maxhealth = 100, int currentHealth = 100, bool alive = true);

        /// <summary>
        /// A Penumbra light instance is taken as parameter and
        /// then added to the entity's component list.
        /// </summary>
        /// <param name="light"> penumbra light instance </param>
        /// <returns></returns>
        EntityBuilder SetLight(Light light);

        /// <summary>
        /// Added to tag a entity as a player entity.
        /// </summary>
        /// <param name="name"> the name of the player</param>
        /// <returns></returns>
        EntityBuilder SetPlayer(string name);

        /// <summary>
        /// Gives the entity a team it will
        /// belong to.
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        EntityBuilder SetTeam(int teamId);

        /// <summary>
        /// An entity with this sound component is able to make different
        /// in-game sounds, eg. movement, death or other sounds.
        /// </summary>
        /// <param name="soundName"></param>
        /// <param name="soundList"></param>
        /// <param name="volume"> playing volume </param>
        /// <param name="soundEffect"></param>
        /// <returns></returns>
        EntityBuilder SetSound(string soundName = null, Dictionary<SoundComponent.SoundBank, SoundEffectInstance> soundList = null, float volume = 1, SoundEffect soundEffect = null);

        /// <summary>
        /// Makes the entity visible on screen.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="isFixed"></param>
        /// <returns></returns>
        EntityBuilder SetRendering(int width, int height, bool isFixed);

        /// <summary>
        /// Entities with the movecomponent are able to move around in the
        /// game with a velocity, acceleration and direction.
        /// </summary>
        /// <param name="maxVelocity"></param>
        /// <param name="acceleration"></param>
        /// <param name="rotationSpeed"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        EntityBuilder SetMovement(float maxVelocity, float acceleration, float rotationSpeed, float direction);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        EntityBuilder SetInertiaDampening(int stabilisingSpeed);

        /// <summary>
        /// Gives the entity a speed penalty
        /// when moving backwards.
        /// </summary>
        /// <returns></returns>
        EntityBuilder SetBackwardsPenalty();

        /// <summary>
        /// Entities that have the camerafollow component added, will be
        /// followed by the game camera so they should not go ofscreen.
        /// The id is which camera to be followed by.
        /// </summary>
        /// <returns></returns>
        EntityBuilder SetCameraFollow(int id);

        /// <summary>
        /// The entitys camera, everything will
        /// be rendered after the camera.
        /// </summary>
        /// <returns></returns>
        EntityBuilder SetCameraView(Viewport view, float minScale, int CameraId);
    }
}