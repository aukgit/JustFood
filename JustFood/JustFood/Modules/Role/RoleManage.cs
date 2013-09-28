using System;
using System.Linq;
using System.Web.Security;
using JustFood.Models;
using JustFood.Modules.Query;

namespace JustFood.Modules.Role {
    public class RoleManage {
        private readonly JustFoodDBEntities db = new JustFoodDBEntities();
        private readonly UserInfo userinfo = new UserInfo();

        /// <summary>
        ///     Add roles by checking if exist or not.
        ///     Note: must call after checking user exist.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="role"></param>
        private void AddRoleByChecking(string log, string role) {
            if (!Roles.IsUserInRole(log, role)) {
                Roles.AddUserToRole(log, role);
            }
        }

        /// <summary>
        ///     Remove roles by checking if exist or not.
        ///     Note: must call after checking user exist.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="role"></param>
        private void ReoveRoleByChecking(string log, string role) {
            if (Roles.IsUserInRole(log, role)) {
                Roles.RemoveUserFromRole(log, role);
            }
        }

        /// <summary>
        ///     Get all underlying roles from this role
        ///     in custom db.
        /// </summary>
        private string[] GetRoles(string roleName) {
            UserRole role = db.UserRoles.Find(roleName);
            string[] roles = null;
            if (role != null) {
                roles = db.UserRoles.Where(n => n.RolePriority <= role.RolePriority)
                          .Select(n => n.UserRoleID)
                          .ToArray();
            }
            return roles;
        }

        /// <summary>
        ///     Verify and add or remove role.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="role"></param>
        /// <param name="status">same as role</param>
        public void VerifyAddRemoveRole(string log, string role, string status) {
            if (String.IsNullOrWhiteSpace(log) || String.IsNullOrWhiteSpace(role)) {
                return;
            }
            role = role.ToLower();
            status = status.ToLower();
            if (role.Equals(status)) {
                VerifyAddRemoveRole(log, role, true);
            } else {
                ReoveRoleByChecking(log, role);
            }
        }

        /// <summary>
        ///     Fastest way to verify and add or remove role.
        ///     Add also all underlying roles.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        public void VerifyAddRemoveRole(string log, string role, bool previousStatus, bool currentStatus) {
            if (String.IsNullOrWhiteSpace(log) || String.IsNullOrWhiteSpace(role)) {
                return;
            }
            bool add = !previousStatus && currentStatus;

            if (add) {
                // add
                string[] roles = GetRoles(role);
                if (roles != null) {
                    foreach (string sRole in roles) {
                        Roles.AddUserToRole(log, sRole);
                    }
                }
            } else {
                // remove
                Roles.RemoveUserFromRole(log, role);
            }
        }

        /// <summary>
        ///     Verify and add or remove role.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        public void VerifyAddRemoveRole(string log, string role, bool status) {
            if (String.IsNullOrWhiteSpace(log) || String.IsNullOrWhiteSpace(role)) {
                return;
            }
            if (status) {
                // add
                if (!Roles.IsUserInRole(log, role)) {
                    string[] roles = GetRoles(role);
                    if (roles != null) {
                        foreach (string sRole in roles) {
                            AddRoleByChecking(log, sRole);
                        }
                    }
                }
            } else {
                // remove
                ReoveRoleByChecking(log, role);
            }
        }

        /// <summary>
        ///     Add all underlying roles
        /// </summary>
        /// <param name="log"></param>
        /// <param name="role"></param>
        public void AddRole(string log, string role) {
            if (String.IsNullOrWhiteSpace(log) || String.IsNullOrWhiteSpace(role)) {
                return;
            }
            if (userinfo.IsUserExist(log)) {
                string[] roles = GetRoles(role);
                if (roles != null) {
                    foreach (string sRole in roles) {
                        AddRoleByChecking(log, sRole);
                    }
                }
            } else {
                throw new Exception("User doesn't exist.");
            }
        }

        /// <summary>
        ///     Remove that specific role only.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="role"></param>
        public void RemoveRole(string log, string role) {
            if (String.IsNullOrWhiteSpace(log) || String.IsNullOrWhiteSpace(role)) {
                return;
            }
            if (userinfo.IsUserExist(log)) {
                RemoveRole(log, role);
            } else {
                throw new Exception("User doesn't exist.");
            }
        }
    }
}