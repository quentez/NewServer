﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache.Posts;
using Server.Lib.ScopeServices;

namespace Server.Lib.Models.Resources.Posts
{
    public class PostPermissions
    {
        public static async Task<PostPermissions> FromCacheAsync(
            IInternalUserLoader internalUserLoader,
            CachePostPermissions cachePostPermissions,
            CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(internalUserLoader, nameof(internalUserLoader));
            Ensure.Argument.IsNotNull(cachePostPermissions, nameof(cachePostPermissions));

            // Load dependencies.
            var userTasks = cachePostPermissions.UserIds?.Select(u => internalUserLoader.FetchAsync(u, cancellationToken)).ToList() ?? new List<Task<User>>();
            var groupTasks = cachePostPermissions.Groups?.Select(g => PostReference.FromCacheAsync(internalUserLoader, g, cancellationToken)).ToList() ?? new List<Task<PostReference>>();

            await Task.WhenAll(
                Task.WhenAll(userTasks),
                Task.WhenAll(groupTasks)
            );

            var users = userTasks.Select(t => t.Result).ToList();
            Ensure.Dependency.IsNotNull(users, nameof(users));

            // Create the resulting post permissions.
            return new PostPermissions
            {
                IsPublic = cachePostPermissions.IsPublic,
                Users = users,
                Groups = groupTasks.Select(t => t.Result).ToList()
            };
        }

        public bool IsPublic { get; set; }
        public List<User> Users { get; set; }
        public List<PostReference> Groups { get; set; }

        public CachePostPermissions ToCache()
        {
            return new CachePostPermissions
            {
                IsPublic = this.IsPublic,
                Groups = this.Groups.Select(g => g.ToCache()).ToList(),
                UserIds = this.Users.Select(u => u.Id).ToList()
            };
        }
    }
}