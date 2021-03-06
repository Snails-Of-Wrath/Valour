﻿using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Valour.Client.Channels;
using Valour.Shared;
using Valour.Shared.Channels;
using Valour.Shared.Planets;
using Valour.Shared.Categories;

namespace Valour.Client.Planets
{
    /*  Valour - A free and secure chat client
     *  Copyright (C) 2020 Vooper Media LLC
     *  This program is subject to the GNU Affero General Public license
     *  A copy of the license should be included - if not, see <http://www.gnu.org/licenses/>
     */

    /// <summary>
    /// This class exists to add client funtionality to the Planet
    /// class. It does not, and should not, have any extra fields or properties.
    /// Just helper methods.
    /// </summary>
    public class ClientPlanet : Planet
    {

        /// <summary>
        /// Returns the generic planet object
        /// </summary>
        public Planet Planet
        {
            get
            {
                return (Planet)this;
            }
        }

        /// <summary>
        /// Returns a ServerPlanet using a Planet as a base
        /// </summary>
        public static ClientPlanet FromBase(Planet planet, IMapper mapper)
        {
            return mapper.Map<ClientPlanet>(planet);
        }

        /// <summary>
        /// Returns the primary channel of the planet
        /// </summary>
        public async Task<ClientPlanetChatChannel> GetPrimaryChannelAsync(IMapper mapper)
        {
            string json = await ClientUserManager.Http.GetStringAsync($"Planet/GetPrimaryChannel?planet_id={Id}" +
                                                                                              $"&userid={ClientUserManager.User.Id}" +
                                                                                              $"&token={ClientUserManager.UserSecretToken}");

            TaskResult<PlanetChatChannel> channelResult = JsonConvert.DeserializeObject<TaskResult<PlanetChatChannel>>(json);

            if (channelResult == null)
            {
                Console.WriteLine($"Failed to retrieve primary channel for planet {Id}.");
                return null;
            }

            if (!channelResult.Success)
            {
                Console.WriteLine($"Failed to retrieve primary channel for planet {Id}: {channelResult.Message}");
            }

            // Map to new
            ClientPlanetChatChannel channel = mapper.Map<ClientPlanetChatChannel>(channelResult.Data);

            return channel;
        }

        /// <summary>
        /// Retrieves and returns categories of a planet by requesting from the server
        /// </summary>
        public async Task<IEnumerable<PlanetCategory>> GetClientPlanetCategoriesAsync(ulong id)
        {
            string json = await ClientUserManager.Http.GetStringAsync($"Category/GetPlanetCategories?planet_id={id}");

            TaskResult<IEnumerable<PlanetCategory>> result = JsonConvert.DeserializeObject<TaskResult<IEnumerable<PlanetCategory>>>(json);

            if (result == null)
            {
                Console.WriteLine("A fatal error occurred retrieving a planet from the server.");
                return null;
            }

            if (!result.Success)
            {
                Console.WriteLine(result.ToString());
                return null;
            }

            return result.Data;
        }

        /// <summary>
        /// Retrieves and returns channels of a planet by requesting from the server
        /// </summary>
        public async Task<IEnumerable<ClientPlanetChatChannel>> GetClientPlanetChannelsAsync(ulong id)
        {
            string json = await ClientUserManager.Http.GetStringAsync($"Channel/GetPlanetChannels?planet_id={id}");

            TaskResult<IEnumerable<ClientPlanetChatChannel>> result = JsonConvert.DeserializeObject<TaskResult<IEnumerable<ClientPlanetChatChannel>>>(json);

            if (result == null)
            {
                Console.WriteLine("A fatal error occurred retrieving a planet from the server.");
                return null;
            }

            if (!result.Success)
            {
                Console.WriteLine(result.ToString());
                return null;
            }

            return result.Data;
        }

        /// <summary>
        /// Attempts to set the name of the planet
        /// </summary>
        public async Task<TaskResult> SetName(string name)
        {
            string json = await ClientUserManager.Http.GetStringAsync($"Planet/SetName?planet_id={Id}&name={name}&token={ClientUserManager.UserSecretToken}");

            TaskResult result = JsonConvert.DeserializeObject<TaskResult>(json);

            if (result.Success)
            {
                Name = name;
            }

            return result;
        }

        /// <summary>
        /// Attempts to set the description of the planet
        /// </summary>
        public async Task<TaskResult> SetDescription(string description)
        {
            string json = await ClientUserManager.Http.GetStringAsync($"Planet/SetDescription?planet_id={Id}&description={description}&token={ClientUserManager.UserSecretToken}");

            TaskResult result = JsonConvert.DeserializeObject<TaskResult>(json);

            if (result.Success)
            {
                Description = description;
            }

            return result;
        }

        /// <summary>
        /// Attempts to set the public value of the planet
        /// </summary>
        public async Task<TaskResult> SetPublic(bool ispublic)
        {
            string json = await ClientUserManager.Http.GetStringAsync($"Planet/SetPublic?planet_id={Id}&ispublic={ispublic}&token={ClientUserManager.UserSecretToken}");

            TaskResult result = JsonConvert.DeserializeObject<TaskResult>(json);

            if (result.Success)
            {
                Public = ispublic;
            }

            return result;
        }

        /// <summary>
        /// Retrieves and returns a client planet by requesting from the server
        /// </summary>
        public static async Task<ClientPlanet> GetClientPlanetAsync(ulong id)
        {
            string json = await ClientUserManager.Http.GetStringAsync($"Planet/GetPlanet?planet_id={id}&auth={ClientUserManager.UserSecretToken}");

            TaskResult<ClientPlanet> result = JsonConvert.DeserializeObject<TaskResult<ClientPlanet>>(json);

            if (result == null)
            {
                Console.WriteLine("A fatal error occurred retrieving a planet from the server.");
                return null;
            }

            if (!result.Success)
            {
                Console.WriteLine(result.ToString());
                return null;
            }

            return result.Data;
        }

        /// <summary>
        /// Deserializes json
        /// </summary>
        public static ClientPlanet Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<ClientPlanet>(json);
        }
    }
}
