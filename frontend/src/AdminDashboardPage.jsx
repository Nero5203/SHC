import { useCallback, useEffect, useMemo, useState } from "react";
import {
  Bell,
  CreditCard,
  LayoutDashboard,
  Plus,
  Search,
  Server,
  Settings,
  Shield,
  ShoppingCart,
  UserCircle2,
  Users
} from "./icons.jsx";
import {
  clearAuth,
  defaultApiUrl,
  deleteJson,
  getJson,
  getPermissionsFromToken,
  getUserNameFromToken,
  isAdminToken,
  isAuthenticated,
  postJson,
  putJson
} from "./apiClient.js";

const nodeStatusOptions = [
  { value: "0", label: "Online" },
  { value: "1", label: "Offline" },
  { value: "2", label: "Maintenance" },
  { value: "3", label: "Degraded" },
  { value: "4", label: "Full" }
];

const registrationModeOptions = [
  { value: "0", label: "Disabled" },
  { value: "1", label: "Enabled" },
  { value: "2", label: "Invite Only" }
];

const purchaseStatusOptions = [
  { value: "0", label: "Pending" },
  { value: "1", label: "Paid" },
  { value: "2", label: "Failed" },
  { value: "3", label: "Cancelled" },
  { value: "4", label: "Refunded" }
];

const billingIntervalOptions = [
  { value: "0", label: "Monthly" },
  { value: "1", label: "Yearly" }
];

const subscriptionStatusOptions = [
  { value: "0", label: "Trialing" },
  { value: "1", label: "Active" },
  { value: "2", label: "Past Due" },
  { value: "3", label: "Cancelled" },
  { value: "4", label: "Expired" },
  { value: "5", label: "Pending" }
];

const emptyStorageNodeForm = {
  storageNodeId: "",
  name: "",
  hostname: "",
  ipAddress: "",
  port: "",
  basePath: "",
  totalCapacityBytes: ""
};

const emptySubscriptionPlanForm = {
  subscriptionPlanId: "",
  name: "",
  description: "",
  price: "",
  currency: "EUR",
  billingInterval: "0",
  storageLimitBytes: "",
  maxFileSizeBytes: "",
  isActive: true
};

const emptySystemSettingsForm = {
  maxFileSizeInBytes: "",
  defaultUserStorageQuotaInBytes: "",
  allowedFileExtensions: "",
  allowPublicLinkSharing: false,
  defaultLinkExpirationInDays: "",
  enforceLinkPasswordProtection: false,
  registrationMode: "1",
  twoFactorAuthRequired: false,
  maxLoginAttempts: "",
  minimumPasswordLength: "",
  requireUppercasePassword: false,
  requireNumberPassword: false,
  requireSpecialCharacterPassword: false,
  trashRetentionInDays: ""
};

const adminModules = [
  {
    key: "users",
    title: "Users",
    area: "Accounts",
    permissions: ["User.Manage", "System.Admin"],
    route: "/api/users",
    countPath: "/api/users",
    description: "Manage profiles, settings, deletion, and account ownership.",
    note: "The admin list endpoint is available now."
  },
  {
    key: "storage",
    title: "Storage Nodes",
    area: "Infrastructure",
    permissions: ["Node.Manage", "System.Admin"],
    route: "/api/storage-nodes",
    countPath: "/api/storage-nodes",
    description: "Register laptop nodes, update heartbeat, and manage node capacity."
  },
  {
    key: "settings",
    title: "System Settings",
    area: "Platform",
    permissions: ["System.Admin"],
    route: "/api/system-settings",
    countPath: "/api/system-settings/all",
    description: "Control file limits, registration rules, link defaults, and trash retention."
  },
  {
    key: "purchases",
    title: "Purchases",
    area: "Billing",
    permissions: ["System.Admin"],
    route: "/api/purchases",
    countPath: "/api/purchases",
    description: "Review purchases, update payment status, open invoices, and create checkout links."
  },
  {
    key: "subscriptions",
    title: "Subscriptions",
    area: "Billing",
    permissions: ["System.Admin"],
    route: "/api/subscriptions",
    countPath: "/api/subscriptions/plans",
    description: "Manage subscription plans and user subscription records."
  },
  {
    key: "audit",
    title: "Audit Logs",
    area: "Security",
    permissions: ["System.Admin"],
    route: "/api/audit-logs",
    countPath: "/api/audit-logs",
    description: "Review important permission and system actions."
  }
];

const dashboardModule = {
  key: "dashboard",
  title: "Dashboard",
  area: "Overview",
  route: "Multiple",
  description: "A live overview of users, storage, billing, and platform status."
};

function AdminDashboardPage({ onLogout }) {
  const apiUrl = localStorage.getItem("shc.apiUrl") || defaultApiUrl;
  const [activeModuleKey, setActiveModuleKey] = useState("dashboard");
  const [counts, setCounts] = useState({});
  const [loadingCounts, setLoadingCounts] = useState(false);
  const [adminUsers, setAdminUsers] = useState([]);
  const [userDirectory, setUserDirectory] = useState([]);
  const [availableRoles, setAvailableRoles] = useState([]);
  const [userRoleMap, setUserRoleMap] = useState({});
  const [selectedRoleByUser, setSelectedRoleByUser] = useState({});
  const [loadingUsers, setLoadingUsers] = useState(false);
  const [usersError, setUsersError] = useState("");
  const [roleActionKey, setRoleActionKey] = useState("");
  const [storageNodes, setStorageNodes] = useState([]);
  const [storageNodeForm, setStorageNodeForm] = useState(emptyStorageNodeForm);
  const [heartbeatDrafts, setHeartbeatDrafts] = useState({});
  const [loadingStorageNodes, setLoadingStorageNodes] = useState(false);
  const [storageNodesError, setStorageNodesError] = useState("");
  const [storageActionKey, setStorageActionKey] = useState("");
  const [purchases, setPurchases] = useState([]);
  const [purchaseIdSearch, setPurchaseIdSearch] = useState("");
  const [purchaseUserIdSearch, setPurchaseUserIdSearch] = useState("");
  const [invoiceDetails, setInvoiceDetails] = useState(null);
  const [checkoutDetails, setCheckoutDetails] = useState(null);
  const [loadingPurchases, setLoadingPurchases] = useState(false);
  const [purchasesError, setPurchasesError] = useState("");
  const [purchaseActionKey, setPurchaseActionKey] = useState("");
  const [subscriptionPlans, setSubscriptionPlans] = useState([]);
  const [subscriptions, setSubscriptions] = useState([]);
  const [subscriptionPlanForm, setSubscriptionPlanForm] = useState(emptySubscriptionPlanForm);
  const [subscriptionUserIdSearch, setSubscriptionUserIdSearch] = useState("");
  const [selectedPlanBySubscription, setSelectedPlanBySubscription] = useState({});
  const [loadingSubscriptions, setLoadingSubscriptions] = useState(false);
  const [subscriptionsError, setSubscriptionsError] = useState("");
  const [subscriptionActionKey, setSubscriptionActionKey] = useState("");
  const [systemSettings, setSystemSettings] = useState(null);
  const [systemSettingsForm, setSystemSettingsForm] = useState(emptySystemSettingsForm);
  const [loadingSystemSettings, setLoadingSystemSettings] = useState(false);
  const [systemSettingsError, setSystemSettingsError] = useState("");
  const [savingSystemSettings, setSavingSystemSettings] = useState(false);
  const [allowedExtensionDraft, setAllowedExtensionDraft] = useState("");

  const permissions = getPermissionsFromToken();
  const isAdmin = isAdminToken();
  const displayName = cleanDisplayName(getUserNameFromToken());

  const visibleModules = useMemo(() => {
    if (isAdmin) return adminModules;

    const permissionSet = new Set(permissions.map((permission) => permission.toLowerCase()));
    return adminModules.filter((module) =>
      module.permissions.some((permission) => permissionSet.has(permission.toLowerCase()))
    );
  }, [isAdmin, permissions]);

  const navigationModules = useMemo(
    () => [dashboardModule, ...visibleModules],
    [visibleModules]
  );
  const activeModule = navigationModules.find((module) => module.key === activeModuleKey) ?? navigationModules[0];
  const allowedExtensionList = parseAllowedFileExtensions(systemSettingsForm.allowedFileExtensions);
  const usersById = useMemo(
    () => Object.fromEntries(userDirectory.map((user) => [getValue(user, "userId", "UserId"), user])),
    [userDirectory]
  );

  const loadUsersWithRoles = useCallback(async () => {
    setLoadingUsers(true);
    setUsersError("");

    try {
      const [users, roleList] = await Promise.all([
        getJson(apiUrl, "/api/users"),
        getJson(apiUrl, "/api/roles")
      ]);

      const safeUsers = Array.isArray(users) ? users : [];
      const safeRoles = Array.isArray(roleList) ? roleList : [];

      setAdminUsers(safeUsers);
      setUserDirectory(safeUsers);
      setAvailableRoles(safeRoles);

      const userRoleEntries = await Promise.all(
        safeUsers.map(async (user) => {
          const userId = getValue(user, "userId", "UserId");
          if (!userId) return ["", []];

          try {
            const userRoles = await getJson(apiUrl, `/api/roles/user/${userId}`);
            return [userId, Array.isArray(userRoles) ? userRoles : []];
          } catch (error) {
            return [userId, []];
          }
        })
      );

      setUserRoleMap(Object.fromEntries(userRoleEntries.filter(([userId]) => userId)));
    } catch (error) {
      setUsersError(error.message);
    } finally {
      setLoadingUsers(false);
    }
  }, [apiUrl]);

  const loadUserDirectory = useCallback(async () => {
    try {
      const users = await getJson(apiUrl, "/api/users");
      setUserDirectory(Array.isArray(users) ? users : []);
    } catch (error) {
      // The billing screens can still render ids if user lookup fails.
    }
  }, [apiUrl]);

  async function handleAssignRole(userId) {
    const roleId = selectedRoleByUser[userId];
    if (!roleId) return;

    const actionKey = `${userId}:${roleId}:assign`;
    setRoleActionKey(actionKey);

    try {
      await postJson(apiUrl, `/api/roles/${roleId}/users/${userId}`, {});
      setSelectedRoleByUser((current) => ({ ...current, [userId]: "" }));
      await loadUsersWithRoles();
    } catch (error) {
      setUsersError(error.message);
    } finally {
      setRoleActionKey("");
    }
  }

  async function handleRemoveRole(userId, roleId) {
    const actionKey = `${userId}:${roleId}:remove`;
    setRoleActionKey(actionKey);

    try {
      await deleteJson(apiUrl, `/api/roles/${roleId}/users/${userId}`);
      await loadUsersWithRoles();
    } catch (error) {
      setUsersError(error.message);
    } finally {
      setRoleActionKey("");
    }
  }

  const loadStorageNodes = useCallback(async () => {
    setLoadingStorageNodes(true);
    setStorageNodesError("");

    try {
      const data = await getJson(apiUrl, "/api/storage-nodes");
      const safeNodes = Array.isArray(data) ? data : [];

      setStorageNodes(safeNodes);
      setHeartbeatDrafts(
        Object.fromEntries(
          safeNodes.map((node) => {
            const nodeId = getValue(node, "storageNodeId", "StorageNodeId");
            return [
              nodeId,
              {
                totalCapacityBytes: getValue(node, "totalCapacityBytes", "TotalCapacityBytes"),
                usedCapacityBytes: getValue(node, "usedCapacityBytes", "UsedCapacityBytes"),
                status: getNodeStatusValue(node)
              }
            ];
          })
        )
      );
    } catch (error) {
      setStorageNodesError(error.message);
    } finally {
      setLoadingStorageNodes(false);
    }
  }, [apiUrl]);

  async function handleSaveStorageNode(event) {
    event.preventDefault();
    const isEditing = !!storageNodeForm.storageNodeId;
    const actionKey = isEditing ? `${storageNodeForm.storageNodeId}:edit` : "create";

    setStorageActionKey(actionKey);
    setStorageNodesError("");

    const body = {
      Name: storageNodeForm.name.trim(),
      Hostname: storageNodeForm.hostname.trim(),
      IpAddress: storageNodeForm.ipAddress.trim(),
      Port: Number(storageNodeForm.port),
      BasePath: storageNodeForm.basePath.trim(),
      TotalCapacityBytes: Number(storageNodeForm.totalCapacityBytes)
    };

    try {
      if (isEditing) {
        await putJson(apiUrl, `/api/storage-nodes/${storageNodeForm.storageNodeId}`, body);
      } else {
        await postJson(apiUrl, "/api/storage-nodes", body);
      }

      setStorageNodeForm(emptyStorageNodeForm);
      await loadStorageNodes();
    } catch (error) {
      setStorageNodesError(error.message);
    } finally {
      setStorageActionKey("");
    }
  }

  function handleEditStorageNode(node) {
    setStorageNodeForm({
      storageNodeId: getValue(node, "storageNodeId", "StorageNodeId"),
      name: getValue(node, "name", "Name"),
      hostname: getValue(node, "hostname", "Hostname"),
      ipAddress: getValue(node, "ipAddress", "IpAddress"),
      port: getValue(node, "port", "Port"),
      basePath: getValue(node, "basePath", "BasePath"),
      totalCapacityBytes: getValue(node, "totalCapacityBytes", "TotalCapacityBytes")
    });
  }

  async function handleUpdateStorageNodeStatus(nodeId, status) {
    const actionKey = `${nodeId}:status`;
    setStorageActionKey(actionKey);
    setStorageNodesError("");

    try {
      await putJson(apiUrl, `/api/storage-nodes/${nodeId}/status`, {
        Status: Number(status)
      });
      await loadStorageNodes();
    } catch (error) {
      setStorageNodesError(error.message);
    } finally {
      setStorageActionKey("");
    }
  }

  async function handleStorageNodeHeartbeat(nodeId) {
    const draft = heartbeatDrafts[nodeId];
    if (!draft) return;

    const actionKey = `${nodeId}:heartbeat`;
    setStorageActionKey(actionKey);
    setStorageNodesError("");

    try {
      await putJson(apiUrl, `/api/storage-nodes/${nodeId}/heartbeat`, {
        TotalCapacityBytes: Number(draft.totalCapacityBytes),
        UsedCapacityBytes: Number(draft.usedCapacityBytes),
        Status: Number(draft.status)
      });
      await loadStorageNodes();
    } catch (error) {
      setStorageNodesError(error.message);
    } finally {
      setStorageActionKey("");
    }
  }

  const loadPurchases = useCallback(async () => {
    setLoadingPurchases(true);
    setPurchasesError("");

    try {
      const [data] = await Promise.all([
        getJson(apiUrl, "/api/purchases"),
        userDirectory.length === 0 ? loadUserDirectory() : Promise.resolve()
      ]);
      setPurchases(Array.isArray(data) ? data : []);
    } catch (error) {
      setPurchasesError(error.message);
    } finally {
      setLoadingPurchases(false);
    }
  }, [apiUrl, loadUserDirectory, userDirectory.length]);

  async function handleFindPurchase(event) {
    event.preventDefault();
    const purchaseId = purchaseIdSearch.trim();
    if (!purchaseId) return;

    setLoadingPurchases(true);
    setPurchasesError("");

    try {
      const data = await getJson(apiUrl, `/api/purchases/${purchaseId}`);
      setPurchases(data ? [data] : []);
    } catch (error) {
      setPurchasesError(error.message);
      setPurchases([]);
    } finally {
      setLoadingPurchases(false);
    }
  }

  async function handleFindUserPurchases(event) {
    event.preventDefault();
    const userId = resolveUserIdFromSearch(purchaseUserIdSearch, userDirectory);
    if (!userId) {
      setPurchasesError("User was not found. Use username, email, or user id.");
      setPurchases([]);
      return;
    }

    setLoadingPurchases(true);
    setPurchasesError("");

    try {
      const data = await getJson(apiUrl, `/api/purchases/user/${userId}`);
      setPurchases(Array.isArray(data) ? data : []);
    } catch (error) {
      setPurchasesError(error.message);
      setPurchases([]);
    } finally {
      setLoadingPurchases(false);
    }
  }

  async function handleUpdatePurchaseStatus(purchaseId, status) {
    const actionKey = `${purchaseId}:status`;
    setPurchaseActionKey(actionKey);
    setPurchasesError("");

    try {
      const updatedPurchase = await putJson(apiUrl, `/api/purchases/${purchaseId}/status`, {
        Status: Number(status)
      });

      setPurchases((current) =>
        current.map((purchase) =>
          getValue(purchase, "purchaseId", "PurchaseId") === purchaseId ? updatedPurchase : purchase
        )
      );
    } catch (error) {
      setPurchasesError(error.message);
    } finally {
      setPurchaseActionKey("");
    }
  }

  async function handleLoadInvoice(purchaseId) {
    const actionKey = `${purchaseId}:invoice`;
    setPurchaseActionKey(actionKey);
    setPurchasesError("");
    setInvoiceDetails(null);

    try {
      const data = await getJson(apiUrl, `/api/purchases/${purchaseId}/invoice`);
      setInvoiceDetails(data);
    } catch (error) {
      setPurchasesError(error.message);
    } finally {
      setPurchaseActionKey("");
    }
  }

  async function handleCreateCheckout(purchaseId) {
    const actionKey = `${purchaseId}:checkout`;
    setPurchaseActionKey(actionKey);
    setPurchasesError("");
    setCheckoutDetails(null);

    try {
      const baseUrl = window.location.origin;
      const data = await postJson(apiUrl, `/api/purchases/${purchaseId}/checkout`, {
        SuccessUrl: `${baseUrl}/payment-success?purchaseId=${purchaseId}`,
        CancelUrl: `${baseUrl}/payment-cancelled?purchaseId=${purchaseId}`
      });

      setCheckoutDetails(data);
    } catch (error) {
      setPurchasesError(error.message);
    } finally {
      setPurchaseActionKey("");
    }
  }

  const loadSubscriptionDashboard = useCallback(async () => {
    setLoadingSubscriptions(true);
    setSubscriptionsError("");

    try {
      const [plansData, subscriptionsData] = await Promise.all([
        getJson(apiUrl, "/api/subscriptions/plans?activeOnly=false"),
        getJson(apiUrl, "/api/subscriptions"),
        userDirectory.length === 0 ? loadUserDirectory() : Promise.resolve()
      ]);

      const safePlans = Array.isArray(plansData) ? plansData : [];
      const safeSubscriptions = Array.isArray(subscriptionsData) ? subscriptionsData : [];

      setSubscriptionPlans(safePlans);
      setSubscriptions(safeSubscriptions);
      setSelectedPlanBySubscription(
        Object.fromEntries(
          safeSubscriptions.map((subscription) => [
            getValue(subscription, "subscriptionId", "SubscriptionId"),
            getValue(subscription, "subscriptionPlanId", "SubscriptionPlanId")
          ])
        )
      );
    } catch (error) {
      setSubscriptionsError(error.message);
    } finally {
      setLoadingSubscriptions(false);
    }
  }, [apiUrl, loadUserDirectory, userDirectory.length]);

  async function handleSaveSubscriptionPlan(event) {
    event.preventDefault();
    const isEditing = !!subscriptionPlanForm.subscriptionPlanId;
    const actionKey = isEditing ? `${subscriptionPlanForm.subscriptionPlanId}:plan` : "create-plan";

    setSubscriptionActionKey(actionKey);
    setSubscriptionsError("");

    const body = {
      Name: subscriptionPlanForm.name.trim(),
      Description: subscriptionPlanForm.description.trim() || null,
      Price: Number(subscriptionPlanForm.price),
      Currency: subscriptionPlanForm.currency.trim().toUpperCase(),
      BillingInterval: Number(subscriptionPlanForm.billingInterval),
      StorageLimitBytes: Number(subscriptionPlanForm.storageLimitBytes),
      MaxFileSizeBytes: toNullableNumber(subscriptionPlanForm.maxFileSizeBytes),
      IsActive: subscriptionPlanForm.isActive
    };

    try {
      if (isEditing) {
        await putJson(apiUrl, `/api/subscriptions/plans/${subscriptionPlanForm.subscriptionPlanId}`, body);
      } else {
        await postJson(apiUrl, "/api/subscriptions/plans", body);
      }

      setSubscriptionPlanForm(emptySubscriptionPlanForm);
      await loadSubscriptionDashboard();
    } catch (error) {
      setSubscriptionsError(error.message);
    } finally {
      setSubscriptionActionKey("");
    }
  }

  function handleEditSubscriptionPlan(plan) {
    setSubscriptionPlanForm(mapSubscriptionPlanToForm(plan));
  }

  async function handleFindUserSubscriptions(event) {
    event.preventDefault();
    const userId = resolveUserIdFromSearch(subscriptionUserIdSearch, userDirectory);
    if (!userId) {
      setSubscriptionsError("User was not found. Use username, email, or user id.");
      setSubscriptions([]);
      return;
    }

    setLoadingSubscriptions(true);
    setSubscriptionsError("");

    try {
      const data = await getJson(apiUrl, `/api/subscriptions/user/${userId}`);
      const safeSubscriptions = Array.isArray(data) ? data : [];
      setSubscriptions(safeSubscriptions);
      setSelectedPlanBySubscription(
        Object.fromEntries(
          safeSubscriptions.map((subscription) => [
            getValue(subscription, "subscriptionId", "SubscriptionId"),
            getValue(subscription, "subscriptionPlanId", "SubscriptionPlanId")
          ])
        )
      );
    } catch (error) {
      setSubscriptionsError(error.message);
      setSubscriptions([]);
    } finally {
      setLoadingSubscriptions(false);
    }
  }

  async function handleChangeSubscriptionPlan(subscriptionId) {
    const subscriptionPlanId = selectedPlanBySubscription[subscriptionId];
    if (!subscriptionPlanId) return;

    const actionKey = `${subscriptionId}:change-plan`;
    setSubscriptionActionKey(actionKey);
    setSubscriptionsError("");

    try {
      const updatedSubscription = await putJson(apiUrl, `/api/subscriptions/${subscriptionId}/plan`, {
        SubscriptionPlanId: subscriptionPlanId
      });

      setSubscriptions((current) =>
        current.map((subscription) =>
          getValue(subscription, "subscriptionId", "SubscriptionId") === subscriptionId
            ? updatedSubscription
            : subscription
        )
      );
    } catch (error) {
      setSubscriptionsError(error.message);
    } finally {
      setSubscriptionActionKey("");
    }
  }

  const loadSystemSettings = useCallback(async () => {
    setLoadingSystemSettings(true);
    setSystemSettingsError("");

    try {
      const data = await getJson(apiUrl, "/api/system-settings");
      setSystemSettings(data);
      setSystemSettingsForm(mapSystemSettingsToForm(data));
    } catch (error) {
      setSystemSettingsError(error.message);
    } finally {
      setLoadingSystemSettings(false);
    }
  }, [apiUrl]);

  async function handleSaveSystemSettings(event) {
    event.preventDefault();
    setSavingSystemSettings(true);
    setSystemSettingsError("");

    const body = {
      MaxFileSizeInBytes: toNullableNumber(systemSettingsForm.maxFileSizeInBytes),
      DefaultUserStorageQuotaInBytes: toNullableNumber(systemSettingsForm.defaultUserStorageQuotaInBytes),
      AllowedFileExtensions: systemSettingsForm.allowedFileExtensions.trim(),
      AllowPublicLinkSharing: systemSettingsForm.allowPublicLinkSharing,
      DefaultLinkExpirationInDays: toNullableNumber(systemSettingsForm.defaultLinkExpirationInDays),
      EnforceLinkPasswordProtection: systemSettingsForm.enforceLinkPasswordProtection,
      RegistrationMode: toNullableNumber(systemSettingsForm.registrationMode),
      TwoFactorAuthRequired: systemSettingsForm.twoFactorAuthRequired,
      MaxLoginAttempts: toNullableNumber(systemSettingsForm.maxLoginAttempts),
      MinimumPasswordLength: toNullableNumber(systemSettingsForm.minimumPasswordLength),
      RequireUppercasePassword: systemSettingsForm.requireUppercasePassword,
      RequireNumberPassword: systemSettingsForm.requireNumberPassword,
      RequireSpecialCharacterPassword: systemSettingsForm.requireSpecialCharacterPassword,
      TrashRetentionInDays: toNullableNumber(systemSettingsForm.trashRetentionInDays)
    };

    try {
      const updatedSettings = await putJson(apiUrl, "/api/system-settings", body);
      setSystemSettings(updatedSettings);
      setSystemSettingsForm(mapSystemSettingsToForm(updatedSettings));
    } catch (error) {
      setSystemSettingsError(error.message);
    } finally {
      setSavingSystemSettings(false);
    }
  }

  function handleAddAllowedExtension(event) {
    event.preventDefault();

    const newExtensions = normalizeExtensionInputs(allowedExtensionDraft);
    if (newExtensions.length === 0) return;

    const mergedExtensions = Array.from(new Set([...allowedExtensionList, ...newExtensions]));

    setSystemSettingsForm((current) => ({
      ...current,
      allowedFileExtensions: mergedExtensions.join(",")
    }));
    setAllowedExtensionDraft("");
  }

  function handleRemoveAllowedExtension(extension) {
    const nextExtensions = allowedExtensionList.filter((item) => item !== extension);

    setSystemSettingsForm((current) => ({
      ...current,
      allowedFileExtensions: nextExtensions.join(",")
    }));
  }

  function handleAllowedExtensionKeyDown(event) {
    if (event.key === "Enter") {
      handleAddAllowedExtension(event);
    }
  }

  async function handleLogout() {
    try {
      const refreshToken = localStorage.getItem("shc.refreshToken");
      if (refreshToken) {
        await postJson(apiUrl, "/api/auth/logout", { RefreshToken: refreshToken }).catch(() => {});
      }
    } finally {
      clearAuth();
      onLogout("login");
    }
  }

  useEffect(() => {
    if (!isAuthenticated()) {
      onLogout("login");
      return;
    }

    if (!isAdminToken()) {
      onLogout("home");
    }
  }, [onLogout]);

  useEffect(() => {
    if (!activeModule && navigationModules.length > 0) {
      setActiveModuleKey(navigationModules[0].key);
    }
  }, [activeModule, navigationModules]);

  useEffect(() => {
    const countableModules = visibleModules.filter((module) => module.countPath);
    if (countableModules.length === 0) return;

    let cancelled = false;
    setLoadingCounts(true);

    Promise.all(
      countableModules.map(async (module) => {
        try {
          const data = await getJson(apiUrl, module.countPath);
          const value = Array.isArray(data) ? data.length : data ? 1 : 0;
          return [module.key, { status: "ok", value }];
        } catch (error) {
          return [module.key, { status: "error", value: error.message }];
        }
      })
    ).then((entries) => {
      if (!cancelled) {
        setCounts(Object.fromEntries(entries));
        setLoadingCounts(false);
      }
    });

    return () => {
      cancelled = true;
    };
  }, [apiUrl, visibleModules]);

  useEffect(() => {
    if (activeModuleKey === "users") {
      loadUsersWithRoles();
    }
  }, [activeModuleKey, loadUsersWithRoles]);

  useEffect(() => {
    if (activeModuleKey === "storage") {
      loadStorageNodes();
    }
  }, [activeModuleKey, loadStorageNodes]);

  useEffect(() => {
    if (activeModuleKey === "purchases") {
      if (userDirectory.length === 0) {
        loadUserDirectory();
      }
      loadPurchases();
    }
  }, [activeModuleKey, loadPurchases, loadUserDirectory, userDirectory.length]);

  useEffect(() => {
    if (activeModuleKey === "subscriptions") {
      if (userDirectory.length === 0) {
        loadUserDirectory();
      }
      loadSubscriptionDashboard();
    }
  }, [activeModuleKey, loadSubscriptionDashboard, loadUserDirectory, userDirectory.length]);

  useEffect(() => {
    if (activeModuleKey === "settings") {
      loadSystemSettings();
    }
  }, [activeModuleKey, loadSystemSettings]);

  useEffect(() => {
    if (activeModuleKey === "dashboard") {
      if (userDirectory.length === 0) {
        loadUserDirectory();
      }
      loadUsersWithRoles();
      loadStorageNodes();
      loadPurchases();
      loadSubscriptionDashboard();
    }
  }, [activeModuleKey, loadPurchases, loadStorageNodes, loadSubscriptionDashboard, loadUserDirectory, loadUsersWithRoles, userDirectory.length]);

  const adminOverviewCards = [
    {
      label: "Authorized modules",
      value: String(visibleModules.length).padStart(2, "0"),
      detail: "Areas currently available to this admin token"
    },
    {
      label: "Users",
      value: formatCount(counts.users, loadingCounts),
      detail: "Registered accounts available in the system"
    },
    {
      label: "Storage nodes",
      value: formatCount(counts.storage, loadingCounts),
      detail: "Infrastructure endpoints connected to storage"
    },
    {
      label: "Purchases",
      value: formatCount(counts.purchases, loadingCounts),
      detail: "Billing records currently stored in the platform"
    }
  ];
  const recentAdminUsers = adminUsers.slice(0, 5);
  const recentPurchases = purchases.slice(0, 5);
  const recentSubscriptions = subscriptions.slice(0, 4);
  const healthyNodesCount = storageNodes.filter((node) => getNodeStatusValue(node) === "0").length;
  const nodeUsagePairs = storageNodes.slice(0, 4).map((node) => {
    const total = Number(getValue(node, "totalCapacityBytes", "TotalCapacityBytes")) || 0;
    const used = Number(getValue(node, "usedCapacityBytes", "UsedCapacityBytes")) || 0;
    const percent = total > 0 ? Math.min((used / total) * 100, 100) : 0;

    return {
      id: getValue(node, "storageNodeId", "StorageNodeId"),
      name: getValue(node, "name", "Name") || "Storage node",
      percent
    };
  });

  return (
    <main className="app-shell">
      <aside className="sidebar sidebar-elevated">
        <div className="brand">
          <div className="brand-mark" aria-hidden="true" />
          <div>
            <strong>SHC DRIVE</strong>
            <span>Admin console</span>
          </div>
        </div>

        <div className="sidebar-copy">
          <p className="sidebar-section-label">Control center</p>
          <p>Operate users, infrastructure, billing, and platform settings from one workspace.</p>
        </div>

        <nav className="module-list workspace-module-list" aria-label="Admin modules">
          {navigationModules.map((module) => {
            const ModuleIcon = getAdminModuleIcon(module.key);

            return (
              <button
                className={`module-item ${activeModule?.key === module.key ? "active" : ""}`}
                key={module.key}
                onClick={() => setActiveModuleKey(module.key)}
                type="button"
              >
                <span className="module-item-icon"><ModuleIcon size={18} /></span>
                <span>{module.title}</span>
                <small>{module.area}</small>
              </button>
            );
          })}
        </nav>

        <div className="sidebar-footer">
          <button className="module-item" onClick={() => onLogout("home")} type="button">
            <span className="module-item-icon"><UserCircle2 size={18} /></span>
            <span>User Dashboard</span>
            <small>Switch</small>
          </button>
          <button className="module-item" onClick={handleLogout} type="button">
            <span className="module-item-icon"><Shield size={18} /></span>
            <span>Logout</span>
            <small>Exit</small>
          </button>
        </div>
      </aside>

      <section className="page">
        <header className="workspace-topbar">
          <div className="workspace-search">
            <Search size={18} />
            <input readOnly value="" placeholder="Search users, files, nodes, and more..." />
          </div>
          <div className="workspace-topbar-actions">
            <button className="primary-button workspace-upload-button" onClick={() => setActiveModuleKey("users")} type="button">
              <Plus size={18} />
              Open Users
            </button>
            <button className="workspace-icon-button" type="button" aria-label="Admin notifications">
              <Bell size={18} />
              <span className="workspace-badge">{visibleModules.length}</span>
            </button>
            <div className="workspace-profile-chip">
              <div className="workspace-profile-avatar">
                <UserCircle2 size={22} />
              </div>
              <div>
                <strong>{displayName || "Admin"}</strong>
                <span>{activeModule?.title || "Dashboard"}</span>
              </div>
            </div>
          </div>
        </header>

        <section className="workspace-page-heading">
          <div>
            <p className="eyebrow">Admin Dashboard</p>
            <h1>{displayName ? `Welcome back, ${displayName}.` : "Welcome back, Admin."}</h1>
            <p>
              {activeModule?.description || "Monitor the cloud workspace, review operational activity, and move between the modules your token is allowed to manage."}
            </p>
          </div>
        </section>

        <section className="dashboard-hero dashboard-hero-admin">
          <div className="hero-metric-grid">
            {adminOverviewCards.map((card) => (
              <article className="hero-metric-card" key={card.label}>
                <span>{card.label}</span>
                <strong>{card.value}</strong>
                <small>{card.detail}</small>
              </article>
            ))}
          </div>
        </section>

        <section className="content-grid admin-dashboard-grid dashboard-main-grid">
          <section className="admin-main">
            {activeModule?.key === "dashboard" && (
              <>
                <div className="admin-module-grid">
                  {visibleModules.map((module) => (
                    <article className="panel module-card selected" key={module.key}>
                      <div>
                        <span className="module-area">{module.area}</span>
                        <h2>{module.title}</h2>
                        <p>{module.description}</p>
                      </div>
                      <div className="module-meta">
                        <span>{module.route}</span>
                        <strong>{formatCount(counts[module.key], loadingCounts)}</strong>
                      </div>
                    </article>
                  ))}
                </div>

                <div className="dashboard-split-grid">
                  <section className="panel dashboard-list-panel">
                    <div className="dashboard-section-header">
                      <div>
                        <h2>Recent User Signups</h2>
                        <p>Accounts currently available in your admin view.</p>
                      </div>
                      <button className="dashboard-link-button" onClick={() => setActiveModuleKey("users")} type="button">
                        Open users
                      </button>
                    </div>
                    {recentAdminUsers.length === 0 ? (
                      <p className="empty-text">No users loaded yet.</p>
                    ) : (
                      <div className="dashboard-mini-list">
                        {recentAdminUsers.map((user) => (
                          <article className="dashboard-mini-row" key={getValue(user, "userId", "UserId")}>
                            <div>
                              <strong>{getUserDisplayName(user)}</strong>
                              <span>{getValue(user, "email", "Email")}</span>
                            </div>
                            <span>{getValue(user, "username", "Username")}</span>
                          </article>
                        ))}
                      </div>
                    )}
                  </section>

                  <section className="panel dashboard-list-panel">
                    <div className="dashboard-section-header">
                      <div>
                        <h2>Recent Purchases</h2>
                        <p>Latest billing records currently returned by the backend.</p>
                      </div>
                      <button className="dashboard-link-button" onClick={() => setActiveModuleKey("purchases")} type="button">
                        Open purchases
                      </button>
                    </div>
                    {recentPurchases.length === 0 ? (
                      <p className="empty-text">No purchases loaded yet.</p>
                    ) : (
                      <div className="dashboard-mini-list">
                        {recentPurchases.map((purchase) => (
                          <article className="dashboard-mini-row" key={getValue(purchase, "purchaseId", "PurchaseId")}>
                            <div>
                              <strong>{getUserLookupLabel(getValue(purchase, "userId", "UserId"), usersById)}</strong>
                              <span>{getValue(purchase, "currency", "Currency")} {getValue(purchase, "amount", "Amount")}</span>
                            </div>
                            <span>{getPurchaseStatusLabel(purchase)}</span>
                          </article>
                        ))}
                      </div>
                    )}
                  </section>
                </div>
              </>
            )}

            {activeModule?.key === "users" ? (
              <section className="panel users-admin-panel">
                <div className="users-admin-header">
                  <div>
                    <p className="eyebrow">Accounts</p>
                    <h2>Users</h2>
                    <p>View registered users, see their current roles, and assign or remove roles.</p>
                  </div>
                  <button className="secondary-button" disabled={loadingUsers} onClick={loadUsersWithRoles} type="button">
                    {loadingUsers ? "Refreshing..." : "Refresh"}
                  </button>
                </div>

                {usersError && <p className="inline-error">{usersError}</p>}

                {loadingUsers ? (
                  <p className="loading-text">Loading users...</p>
                ) : adminUsers.length === 0 ? (
                  <p className="empty-text">No users found.</p>
                ) : (
                  <div className="users-table-wrap">
                    <table className="users-table">
                      <thead>
                        <tr>
                          <th>User</th>
                          <th>Email</th>
                          <th>Roles</th>
                          <th>Assign Role</th>
                        </tr>
                      </thead>
                      <tbody>
                        {adminUsers.map((user) => {
                          const userId = getValue(user, "userId", "UserId");
                          const userRoles = userRoleMap[userId] ?? [];
                          const assignedRoleIds = new Set(
                            userRoles.map((role) => getValue(role, "roleId", "RoleId"))
                          );
                          const assignableRoles = availableRoles.filter((role) =>
                            !assignedRoleIds.has(getValue(role, "roleId", "RoleId"))
                          );
                          const selectedRoleId = selectedRoleByUser[userId] ?? "";

                          return (
                            <tr key={userId}>
                              <td>
                                <strong>{getUserDisplayName(user)}</strong>
                                <span>{getValue(user, "username", "Username")}</span>
                              </td>
                              <td>{getValue(user, "email", "Email")}</td>
                              <td>
                                <div className="role-chip-list">
                                  {userRoles.length > 0 ? (
                                    userRoles.map((role) => {
                                      const roleId = getValue(role, "roleId", "RoleId");
                                      const roleName = getValue(role, "roleName", "RoleName");
                                      const isRemoving = roleActionKey === `${userId}:${roleId}:remove`;

                                      return (
                                        <span className="role-chip" key={roleId}>
                                          {roleName}
                                          <button
                                            aria-label={`Remove ${roleName}`}
                                            disabled={isRemoving}
                                            onClick={() => handleRemoveRole(userId, roleId)}
                                            type="button"
                                          >
                                            {isRemoving ? "..." : "x"}
                                          </button>
                                        </span>
                                      );
                                    })
                                  ) : (
                                    <span className="muted-text">No role</span>
                                  )}
                                </div>
                              </td>
                              <td>
                                <div className="role-assign-control">
                                  <select
                                    value={selectedRoleId}
                                    onChange={(event) =>
                                      setSelectedRoleByUser((current) => ({
                                        ...current,
                                        [userId]: event.target.value
                                      }))
                                    }
                                  >
                                    <option value="">
                                      {assignableRoles.length > 0 ? "Choose role" : "All roles assigned"}
                                    </option>
                                    {assignableRoles.map((role) => {
                                      const roleId = getValue(role, "roleId", "RoleId");
                                      const roleName = getValue(role, "name", "Name");

                                      return (
                                        <option key={roleId} value={roleId}>
                                          {roleName}
                                        </option>
                                      );
                                    })}
                                  </select>
                                  <button
                                    className="primary-button"
                                    disabled={!selectedRoleId || roleActionKey === `${userId}:${selectedRoleId}:assign`}
                                    onClick={() => handleAssignRole(userId)}
                                    type="button"
                                  >
                                    {roleActionKey === `${userId}:${selectedRoleId}:assign` ? "Saving..." : "Assign"}
                                  </button>
                                </div>
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </div>
                )}
              </section>
            ) : activeModule?.key === "storage" ? (
              <section className="panel storage-admin-panel">
                <div className="storage-admin-header">
                  <div>
                    <p className="eyebrow">Infrastructure</p>
                    <h2>Storage Nodes</h2>
                    <p>Register laptop nodes, edit connection details, update status, and send heartbeat capacity data.</p>
                  </div>
                  <button className="secondary-button" disabled={loadingStorageNodes} onClick={loadStorageNodes} type="button">
                    {loadingStorageNodes ? "Refreshing..." : "Refresh"}
                  </button>
                </div>

                {storageNodesError && <p className="inline-error">{storageNodesError}</p>}

                <form className="storage-node-form" onSubmit={handleSaveStorageNode}>
                  <div className="storage-node-form-header">
                    <div>
                      <h3>{storageNodeForm.storageNodeId ? "Edit Storage Node" : "Add Storage Node"}</h3>
                      <p>{storageNodeForm.storageNodeId ? "Update the selected node details." : "Register one laptop/server as a storage node."}</p>
                    </div>
                    {storageNodeForm.storageNodeId && (
                      <button className="secondary-button" onClick={() => setStorageNodeForm(emptyStorageNodeForm)} type="button">
                        Cancel Edit
                      </button>
                    )}
                  </div>

                  <div className="storage-node-form-grid">
                    <label>
                      Name
                      <input
                        required
                        value={storageNodeForm.name}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, name: event.target.value }))}
                      />
                    </label>
                    <label>
                      Hostname
                      <input
                        required
                        value={storageNodeForm.hostname}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, hostname: event.target.value }))}
                      />
                    </label>
                    <label>
                      IP Address
                      <input
                        required
                        value={storageNodeForm.ipAddress}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, ipAddress: event.target.value }))}
                      />
                    </label>
                    <label>
                      Port
                      <input
                        min="1"
                        required
                        type="number"
                        value={storageNodeForm.port}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, port: event.target.value }))}
                      />
                    </label>
                    <label>
                      Base Path
                      <input
                        required
                        value={storageNodeForm.basePath}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, basePath: event.target.value }))}
                      />
                    </label>
                    <label>
                      Total Capacity Bytes
                      <input
                        min="1"
                        required
                        type="number"
                        value={storageNodeForm.totalCapacityBytes}
                        onChange={(event) => setStorageNodeForm((current) => ({ ...current, totalCapacityBytes: event.target.value }))}
                      />
                    </label>
                  </div>

                  <button className="primary-button" disabled={!!storageActionKey} type="submit">
                    {storageNodeForm.storageNodeId
                      ? storageActionKey ? "Saving..." : "Save Changes"
                      : storageActionKey === "create" ? "Adding..." : "Add Storage Node"}
                  </button>
                </form>

                {loadingStorageNodes ? (
                  <p className="loading-text">Loading storage nodes...</p>
                ) : storageNodes.length === 0 ? (
                  <p className="empty-text">No storage nodes found.</p>
                ) : (
                  <div className="storage-node-list">
                    {storageNodes.map((node) => {
                      const nodeId = getValue(node, "storageNodeId", "StorageNodeId");
                      const totalBytes = Number(getValue(node, "totalCapacityBytes", "TotalCapacityBytes")) || 0;
                      const usedBytes = Number(getValue(node, "usedCapacityBytes", "UsedCapacityBytes")) || 0;
                      const usedPercent = totalBytes > 0 ? Math.min((usedBytes / totalBytes) * 100, 100) : 0;
                      const heartbeatDraft = heartbeatDrafts[nodeId] ?? {
                        totalCapacityBytes: String(totalBytes),
                        usedCapacityBytes: String(usedBytes),
                        status: getNodeStatusValue(node)
                      };

                      return (
                        <article className="storage-node-card" key={nodeId}>
                          <div className="storage-node-card-header">
                            <div>
                              <h3>{getValue(node, "name", "Name")}</h3>
                              <p>{getValue(node, "hostname", "Hostname")} - {getValue(node, "ipAddress", "IpAddress")}:{getValue(node, "port", "Port")}</p>
                            </div>
                            <span className={`node-status status-${getNodeStatusLabel(node).toLowerCase()}`}>
                              {getNodeStatusLabel(node)}
                            </span>
                          </div>

                          <div className="node-capacity">
                            <div className="node-capacity-meta">
                              <span>{formatBytes(usedBytes)} used</span>
                              <span>{formatBytes(totalBytes)} total</span>
                            </div>
                            <div className="storage-bar">
                              <div className="storage-fill" style={{ width: `${usedPercent}%` }}></div>
                            </div>
                          </div>

                          <dl className="node-detail-grid">
                            <div>
                              <dt>Base Path</dt>
                              <dd>{getValue(node, "basePath", "BasePath")}</dd>
                            </div>
                            <div>
                              <dt>Files</dt>
                              <dd>{getValue(node, "fileCount", "FileCount") || "0"}</dd>
                            </div>
                            <div>
                              <dt>Last Heartbeat</dt>
                              <dd>{formatDate(getValue(node, "lastHeartbeatAt", "LastHeartbeatAt"))}</dd>
                            </div>
                            <div>
                              <dt>Updated</dt>
                              <dd>{formatDate(getValue(node, "updatedAt", "UpdatedAt"))}</dd>
                            </div>
                          </dl>

                          <div className="node-actions">
                            <button className="secondary-button" onClick={() => handleEditStorageNode(node)} type="button">
                              Edit
                            </button>
                            <label>
                              Status
                              <select
                                value={getNodeStatusValue(node)}
                                onChange={(event) => handleUpdateStorageNodeStatus(nodeId, event.target.value)}
                                disabled={storageActionKey === `${nodeId}:status`}
                              >
                                {nodeStatusOptions.map((option) => (
                                  <option key={option.value} value={option.value}>{option.label}</option>
                                ))}
                              </select>
                            </label>
                          </div>

                          <div className="heartbeat-form">
                            <label>
                              Used Bytes
                              <input
                                min="0"
                                type="number"
                                value={heartbeatDraft.usedCapacityBytes}
                                onChange={(event) =>
                                  setHeartbeatDrafts((current) => ({
                                    ...current,
                                    [nodeId]: { ...heartbeatDraft, usedCapacityBytes: event.target.value }
                                  }))
                                }
                              />
                            </label>
                            <label>
                              Total Bytes
                              <input
                                min="1"
                                type="number"
                                value={heartbeatDraft.totalCapacityBytes}
                                onChange={(event) =>
                                  setHeartbeatDrafts((current) => ({
                                    ...current,
                                    [nodeId]: { ...heartbeatDraft, totalCapacityBytes: event.target.value }
                                  }))
                                }
                              />
                            </label>
                            <label>
                              Heartbeat Status
                              <select
                                value={heartbeatDraft.status}
                                onChange={(event) =>
                                  setHeartbeatDrafts((current) => ({
                                    ...current,
                                    [nodeId]: { ...heartbeatDraft, status: event.target.value }
                                  }))
                                }
                              >
                                {nodeStatusOptions.map((option) => (
                                  <option key={option.value} value={option.value}>{option.label}</option>
                                ))}
                              </select>
                            </label>
                            <button
                              className="primary-button"
                              disabled={storageActionKey === `${nodeId}:heartbeat`}
                              onClick={() => handleStorageNodeHeartbeat(nodeId)}
                              type="button"
                            >
                              {storageActionKey === `${nodeId}:heartbeat` ? "Sending..." : "Send Heartbeat"}
                            </button>
                          </div>
                        </article>
                      );
                    })}
                  </div>
                )}
              </section>
            ) : activeModule?.key === "purchases" ? (
              <section className="panel purchases-admin-panel">
                <div className="purchases-admin-header">
                  <div>
                    <p className="eyebrow">Billing</p>
                    <h2>Purchases</h2>
                    <p>Review purchases, update status, open invoices, and create checkout links for pending payments.</p>
                  </div>
                  <button className="secondary-button" disabled={loadingPurchases} onClick={loadPurchases} type="button">
                    {loadingPurchases ? "Refreshing..." : "Load All"}
                  </button>
                </div>

                {purchasesError && <p className="inline-error">{purchasesError}</p>}

                <div className="purchase-search-row">
                  <form onSubmit={handleFindPurchase}>
                    <label>
                      Find by Purchase Id
                      <div>
                        <input
                          value={purchaseIdSearch}
                          onChange={(event) => setPurchaseIdSearch(event.target.value)}
                        />
                        <button className="secondary-button" type="submit">Find</button>
                      </div>
                    </label>
                  </form>
                  <form onSubmit={handleFindUserPurchases}>
                    <label>
                      Find by User
                      <div>
                        <input
                          value={purchaseUserIdSearch}
                          onChange={(event) => setPurchaseUserIdSearch(event.target.value)}
                          placeholder="Username, email, or user id"
                        />
                        <button className="secondary-button" type="submit">Find</button>
                      </div>
                    </label>
                  </form>
                </div>

                {checkoutDetails && (
                  <div className="purchase-result-box">
                    <strong>Checkout session created</strong>
                    <a href={getValue(checkoutDetails, "checkoutUrl", "CheckoutUrl")} target="_blank" rel="noreferrer">
                      Open checkout link
                    </a>
                    <span>{getValue(checkoutDetails, "checkoutSessionId", "CheckoutSessionId")}</span>
                  </div>
                )}

                {invoiceDetails && (
                  <div className="purchase-result-box">
                    <strong>Invoice {getValue(invoiceDetails, "invoiceNumber", "InvoiceNumber")}</strong>
                    <span>Total: {formatMoney(getValue(invoiceDetails, "totalAmount", "TotalAmount"), getValue(invoiceDetails, "currency", "Currency"))}</span>
                    <span>Status: {getValue(invoiceDetails, "status", "Status")}</span>
                    <span>Issued: {formatDate(getValue(invoiceDetails, "issuedAt", "IssuedAt"))}</span>
                  </div>
                )}

                {loadingPurchases ? (
                  <p className="loading-text">Loading purchases...</p>
                ) : purchases.length === 0 ? (
                  <p className="empty-text">No purchases found.</p>
                ) : (
                  <div className="users-table-wrap">
                    <table className="users-table purchases-table">
                      <thead>
                        <tr>
                          <th>Purchase</th>
                          <th>User</th>
                          <th>Subscription</th>
                          <th>Amount</th>
                          <th>Status</th>
                          <th>Actions</th>
                        </tr>
                      </thead>
                      <tbody>
                        {purchases.map((purchase) => {
                          const purchaseId = getValue(purchase, "purchaseId", "PurchaseId");
                          const statusValue = getPurchaseStatusValue(purchase);

                          return (
                            <tr key={purchaseId}>
                              <td>
                                <strong>{purchaseId.slice(0, 8)}...</strong>
                                <span>{formatDate(getValue(purchase, "purchasedAt", "PurchasedAt"))}</span>
                              </td>
                              <td>
                                <strong>{getUserLookupLabel(getValue(purchase, "userId", "UserId"), usersById)}</strong>
                                <span>{getValue(purchase, "userId", "UserId")}</span>
                              </td>
                              <td>{getValue(purchase, "subscriptionId", "SubscriptionId")}</td>
                              <td>{formatMoney(getValue(purchase, "amount", "Amount"), getValue(purchase, "currency", "Currency"))}</td>
                              <td>
                                <span className={`purchase-status status-${getPurchaseStatusLabel(purchase).toLowerCase()}`}>
                                  {getPurchaseStatusLabel(purchase)}
                                </span>
                              </td>
                              <td>
                                <div className="purchase-actions">
                                  <select
                                    value={statusValue}
                                    disabled={purchaseActionKey === `${purchaseId}:status`}
                                    onChange={(event) => handleUpdatePurchaseStatus(purchaseId, event.target.value)}
                                  >
                                    {purchaseStatusOptions.map((option) => (
                                      <option key={option.value} value={option.value}>{option.label}</option>
                                    ))}
                                  </select>
                                  <button
                                    className="secondary-button"
                                    disabled={purchaseActionKey === `${purchaseId}:invoice`}
                                    onClick={() => handleLoadInvoice(purchaseId)}
                                    type="button"
                                  >
                                    {purchaseActionKey === `${purchaseId}:invoice` ? "Loading..." : "Invoice"}
                                  </button>
                                  <button
                                    className="secondary-button"
                                    disabled={purchaseActionKey === `${purchaseId}:checkout`}
                                    onClick={() => handleCreateCheckout(purchaseId)}
                                    type="button"
                                  >
                                    {purchaseActionKey === `${purchaseId}:checkout` ? "Creating..." : "Checkout"}
                                  </button>
                                </div>
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </div>
                )}
              </section>
            ) : activeModule?.key === "subscriptions" ? (
              <section className="panel subscriptions-admin-panel">
                <div className="subscriptions-admin-header">
                  <div>
                    <p className="eyebrow">Billing</p>
                    <h2>Subscriptions</h2>
                    <p>View subscriptions, add or edit plans, and move a subscription to another plan.</p>
                  </div>
                  <button className="secondary-button" disabled={loadingSubscriptions} onClick={loadSubscriptionDashboard} type="button">
                    {loadingSubscriptions ? "Refreshing..." : "Refresh"}
                  </button>
                </div>

                {subscriptionsError && <p className="inline-error">{subscriptionsError}</p>}

                <form className="subscription-plan-form" onSubmit={handleSaveSubscriptionPlan}>
                  <div className="subscription-plan-form-header">
                    <div>
                      <h3>{subscriptionPlanForm.subscriptionPlanId ? "Edit Plan" : "Add Plan"}</h3>
                      <p>{subscriptionPlanForm.subscriptionPlanId ? "Update the selected subscription plan." : "Create a plan users can subscribe to."}</p>
                    </div>
                    {subscriptionPlanForm.subscriptionPlanId && (
                      <button className="secondary-button" onClick={() => setSubscriptionPlanForm(emptySubscriptionPlanForm)} type="button">
                        Cancel Edit
                      </button>
                    )}
                  </div>

                  <div className="subscription-plan-form-grid">
                    <label>
                      Name
                      <input
                        required
                        value={subscriptionPlanForm.name}
                        onChange={(event) => setSubscriptionPlanForm((current) => ({ ...current, name: event.target.value }))}
                      />
                    </label>
                    <label>
                      Price
                      <input
                        min="0"
                        required
                        step="0.01"
                        type="number"
                        value={subscriptionPlanForm.price}
                        onChange={(event) => setSubscriptionPlanForm((current) => ({ ...current, price: event.target.value }))}
                      />
                    </label>
                    <label>
                      Currency
                      <input
                        maxLength="3"
                        required
                        value={subscriptionPlanForm.currency}
                        onChange={(event) => setSubscriptionPlanForm((current) => ({ ...current, currency: event.target.value }))}
                      />
                    </label>
                    <label>
                      Billing Interval
                      <select
                        value={subscriptionPlanForm.billingInterval}
                        onChange={(event) => setSubscriptionPlanForm((current) => ({ ...current, billingInterval: event.target.value }))}
                      >
                        {billingIntervalOptions.map((option) => (
                          <option key={option.value} value={option.value}>{option.label}</option>
                        ))}
                      </select>
                    </label>
                    <label>
                      Storage Limit Bytes
                      <input
                        min="1"
                        required
                        type="number"
                        value={subscriptionPlanForm.storageLimitBytes}
                        onChange={(event) => setSubscriptionPlanForm((current) => ({ ...current, storageLimitBytes: event.target.value }))}
                      />
                    </label>
                    <label>
                      Max File Size Bytes
                      <input
                        min="1"
                        type="number"
                        value={subscriptionPlanForm.maxFileSizeBytes}
                        onChange={(event) => setSubscriptionPlanForm((current) => ({ ...current, maxFileSizeBytes: event.target.value }))}
                      />
                    </label>
                    <label className="wide-field">
                      Description
                      <input
                        value={subscriptionPlanForm.description}
                        onChange={(event) => setSubscriptionPlanForm((current) => ({ ...current, description: event.target.value }))}
                      />
                    </label>
                    <label className="toggle-row">
                      <input
                        checked={subscriptionPlanForm.isActive}
                        type="checkbox"
                        onChange={(event) => setSubscriptionPlanForm((current) => ({ ...current, isActive: event.target.checked }))}
                      />
                      Active plan
                    </label>
                  </div>

                  <button className="primary-button" disabled={!!subscriptionActionKey} type="submit">
                    {subscriptionPlanForm.subscriptionPlanId
                      ? subscriptionActionKey ? "Saving..." : "Save Plan"
                      : subscriptionActionKey === "create-plan" ? "Adding..." : "Add Plan"}
                  </button>
                </form>

                <section className="subscription-plan-list">
                  <div className="subscription-overview-grid">
                    <article className="subscription-overview-card">
                      <span>Total Plans</span>
                      <strong>{subscriptionPlans.length}</strong>
                      <small>Active and inactive plans</small>
                    </article>
                    <article className="subscription-overview-card">
                      <span>Active Plans</span>
                      <strong>{subscriptionPlans.filter((plan) => getBooleanValue(plan, "isActive", "IsActive")).length}</strong>
                      <small>Visible to users</small>
                    </article>
                    <article className="subscription-overview-card">
                      <span>Subscriptions</span>
                      <strong>{subscriptions.length}</strong>
                      <small>Current loaded records</small>
                    </article>
                    <article className="subscription-overview-card">
                      <span>Pending</span>
                      <strong>{subscriptions.filter((item) => getSubscriptionStatusValue(item) === "5").length}</strong>
                      <small>Waiting for payment</small>
                    </article>
                  </div>

                  <div className="section-heading">
                    <h2>Plans</h2>
                    <p>All active and inactive plans.</p>
                  </div>
                  {subscriptionPlans.length === 0 ? (
                    <p className="empty-text">No subscription plans found.</p>
                  ) : (
                    <div className="subscription-plan-grid">
                      {subscriptionPlans.map((plan) => {
                        const planId = getValue(plan, "subscriptionPlanId", "SubscriptionPlanId");

                        return (
                          <article className="subscription-plan-card" key={planId}>
                            <div>
                              <h3>{getValue(plan, "name", "Name")}</h3>
                              <p>{getValue(plan, "description", "Description") || "No description"}</p>
                            </div>
                            <dl>
                              <div>
                                <dt>Price</dt>
                                <dd>{formatMoney(getValue(plan, "price", "Price"), getValue(plan, "currency", "Currency"))}</dd>
                              </div>
                              <div>
                                <dt>Interval</dt>
                                <dd>{getBillingIntervalLabel(plan)}</dd>
                              </div>
                              <div>
                                <dt>Storage</dt>
                                <dd>{formatBytes(Number(getValue(plan, "storageLimitBytes", "StorageLimitBytes")) || 0)}</dd>
                              </div>
                              <div>
                                <dt>Max File</dt>
                                <dd>{formatBytes(Number(getValue(plan, "maxFileSizeBytes", "MaxFileSizeBytes")) || 0)}</dd>
                              </div>
                            </dl>
                            <div className="subscription-plan-card-footer">
                              <span className={`plan-state ${getBooleanValue(plan, "isActive", "IsActive") ? "active" : "inactive"}`}>
                                {getBooleanValue(plan, "isActive", "IsActive") ? "Active" : "Inactive"}
                              </span>
                              <button className="secondary-button" onClick={() => handleEditSubscriptionPlan(plan)} type="button">
                                Edit
                              </button>
                            </div>
                          </article>
                        );
                      })}
                    </div>
                  )}
                </section>

                <div className="subscription-search-row">
                  <form onSubmit={handleFindUserSubscriptions}>
                    <label>
                      Find Subscriptions By User
                      <div>
                        <input
                          value={subscriptionUserIdSearch}
                          onChange={(event) => setSubscriptionUserIdSearch(event.target.value)}
                          placeholder="Username, email, or user id"
                        />
                        <button className="secondary-button" type="submit">Find</button>
                      </div>
                    </label>
                  </form>
                  <button className="secondary-button" disabled={loadingSubscriptions} onClick={loadSubscriptionDashboard} type="button">
                    Show All Subscriptions
                  </button>
                </div>

                {loadingSubscriptions ? (
                  <p className="loading-text">Loading subscriptions...</p>
                ) : subscriptions.length === 0 ? (
                  <p className="empty-text">No subscriptions found.</p>
                ) : (
                  <div className="users-table-wrap">
                    <table className="users-table subscriptions-table">
                      <thead>
                        <tr>
                          <th>Subscription</th>
                          <th>Users</th>
                          <th>Plan</th>
                          <th>Status</th>
                          <th>Period</th>
                          <th>Change Plan</th>
                        </tr>
                      </thead>
                      <tbody>
                        {subscriptions.map((subscription) => {
                          const subscriptionId = getValue(subscription, "subscriptionId", "SubscriptionId");
                          const selectedPlanId = selectedPlanBySubscription[subscriptionId]
                            ?? getValue(subscription, "subscriptionPlanId", "SubscriptionPlanId");

                          return (
                            <tr key={subscriptionId}>
                              <td>
                                <strong>{subscriptionId.slice(0, 8)}...</strong>
                                <span>{getValue(subscription, "providerSubscriptionId", "ProviderSubscriptionId") || "No provider id"}</span>
                              </td>
                              <td>{getUserIdsText(subscription, usersById)}</td>
                              <td>
                                <strong>{getValue(subscription, "planName", "PlanName")}</strong>
                                <span>{getValue(subscription, "subscriptionPlanId", "SubscriptionPlanId")}</span>
                              </td>
                              <td>
                                <span className={`subscription-status status-${getSubscriptionStatusLabel(subscription).toLowerCase().replace(/\s/g, "-")}`}>
                                  {getSubscriptionStatusLabel(subscription)}
                                </span>
                              </td>
                              <td>
                                <strong>{formatDate(getValue(subscription, "currentPeriodStart", "CurrentPeriodStart"))}</strong>
                                <span>to {formatDate(getValue(subscription, "currentPeriodEnd", "CurrentPeriodEnd"))}</span>
                              </td>
                              <td>
                                <div className="subscription-actions">
                                  <select
                                    value={selectedPlanId}
                                    onChange={(event) => setSelectedPlanBySubscription((current) => ({
                                      ...current,
                                      [subscriptionId]: event.target.value
                                    }))}
                                  >
                                    {subscriptionPlans.map((plan) => {
                                      const planId = getValue(plan, "subscriptionPlanId", "SubscriptionPlanId");
                                      return (
                                        <option key={planId} value={planId}>
                                          {getValue(plan, "name", "Name")}
                                        </option>
                                      );
                                    })}
                                  </select>
                                  <button
                                    className="primary-button"
                                    disabled={!selectedPlanId || subscriptionActionKey === `${subscriptionId}:change-plan`}
                                    onClick={() => handleChangeSubscriptionPlan(subscriptionId)}
                                    type="button"
                                  >
                                    {subscriptionActionKey === `${subscriptionId}:change-plan` ? "Saving..." : "Change"}
                                  </button>
                                </div>
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </div>
                )}
              </section>
            ) : activeModule?.key === "settings" ? (
              <section className="panel settings-admin-panel">
                <div className="settings-admin-header">
                  <div>
                    <p className="eyebrow">Platform</p>
                    <h2>System Settings</h2>
                    <p>Update global file limits, link sharing defaults, registration rules, and trash retention.</p>
                  </div>
                  <button className="secondary-button" disabled={loadingSystemSettings} onClick={loadSystemSettings} type="button">
                    {loadingSystemSettings ? "Refreshing..." : "Refresh"}
                  </button>
                </div>

                {systemSettingsError && <p className="inline-error">{systemSettingsError}</p>}

                {loadingSystemSettings ? (
                  <p className="loading-text">Loading system settings...</p>
                ) : (
                  <form className="settings-form" onSubmit={handleSaveSystemSettings}>
                    <section className="settings-section">
                      <div>
                        <h3>File Limits</h3>
                        <p>Storage rules used by uploads and user accounts.</p>
                      </div>
                      <div className="settings-form-grid">
                        <label>
                          Max File Size Bytes
                          <input
                            min="1"
                            required
                            type="number"
                            value={systemSettingsForm.maxFileSizeInBytes}
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              maxFileSizeInBytes: event.target.value
                            }))}
                          />
                        </label>
                        <label>
                          Default User Quota Bytes
                          <input
                            min="1"
                            required
                            type="number"
                            value={systemSettingsForm.defaultUserStorageQuotaInBytes}
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              defaultUserStorageQuotaInBytes: event.target.value
                            }))}
                          />
                        </label>
                        <div className="wide-field extension-builder">
                          <label>
                            Allowed File Extensions
                            <div className="extension-add-row">
                              <input
                                value={allowedExtensionDraft}
                                onChange={(event) => setAllowedExtensionDraft(event.target.value)}
                                onKeyDown={handleAllowedExtensionKeyDown}
                                placeholder="jpg, png, pdf"
                              />
                              <button className="secondary-button" onClick={handleAddAllowedExtension} type="button">
                                Add
                              </button>
                            </div>
                          </label>

                          <div className="extension-chip-list">
                            {allowedExtensionList.length > 0 ? (
                              allowedExtensionList.map((extension) => (
                                <span className="extension-chip" key={extension}>
                                  {extension}
                                  <button
                                    aria-label={`Remove ${extension}`}
                                    onClick={() => handleRemoveAllowedExtension(extension)}
                                    type="button"
                                  >
                                    x
                                  </button>
                                </span>
                              ))
                            ) : (
                              <span className="muted-text">No extensions allowed yet.</span>
                            )}
                          </div>
                        </div>
                      </div>
                    </section>

                    <section className="settings-section">
                      <div>
                        <h3>Public Links</h3>
                        <p>Defaults for sharing files and folders with links.</p>
                      </div>
                      <div className="settings-form-grid">
                        <label>
                          Default Expiration Days
                          <input
                            min="1"
                            required
                            type="number"
                            value={systemSettingsForm.defaultLinkExpirationInDays}
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              defaultLinkExpirationInDays: event.target.value
                            }))}
                          />
                        </label>
                        <label className="toggle-row">
                          <input
                            checked={systemSettingsForm.allowPublicLinkSharing}
                            type="checkbox"
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              allowPublicLinkSharing: event.target.checked
                            }))}
                          />
                          Allow public link sharing
                        </label>
                        <label className="toggle-row">
                          <input
                            checked={systemSettingsForm.enforceLinkPasswordProtection}
                            type="checkbox"
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              enforceLinkPasswordProtection: event.target.checked
                            }))}
                          />
                          Require link passwords
                        </label>
                      </div>
                    </section>

                    <section className="settings-section">
                      <div>
                        <h3>Registration And Auth</h3>
                        <p>Global rules for signups, login attempts, and password strength.</p>
                      </div>
                      <div className="settings-form-grid">
                        <label>
                          Registration Mode
                          <select
                            value={systemSettingsForm.registrationMode}
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              registrationMode: event.target.value
                            }))}
                          >
                            {registrationModeOptions.map((option) => (
                              <option key={option.value} value={option.value}>{option.label}</option>
                            ))}
                          </select>
                        </label>
                        <label>
                          Max Login Attempts
                          <input
                            min="1"
                            required
                            type="number"
                            value={systemSettingsForm.maxLoginAttempts}
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              maxLoginAttempts: event.target.value
                            }))}
                          />
                        </label>
                        <label>
                          Minimum Password Length
                          <input
                            min="1"
                            required
                            type="number"
                            value={systemSettingsForm.minimumPasswordLength}
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              minimumPasswordLength: event.target.value
                            }))}
                          />
                        </label>
                        <label className="toggle-row">
                          <input
                            checked={systemSettingsForm.twoFactorAuthRequired}
                            type="checkbox"
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              twoFactorAuthRequired: event.target.checked
                            }))}
                          />
                          Require two-factor auth
                        </label>
                        <label className="toggle-row">
                          <input
                            checked={systemSettingsForm.requireUppercasePassword}
                            type="checkbox"
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              requireUppercasePassword: event.target.checked
                            }))}
                          />
                          Require uppercase
                        </label>
                        <label className="toggle-row">
                          <input
                            checked={systemSettingsForm.requireNumberPassword}
                            type="checkbox"
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              requireNumberPassword: event.target.checked
                            }))}
                          />
                          Require number
                        </label>
                        <label className="toggle-row">
                          <input
                            checked={systemSettingsForm.requireSpecialCharacterPassword}
                            type="checkbox"
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              requireSpecialCharacterPassword: event.target.checked
                            }))}
                          />
                          Require special character
                        </label>
                      </div>
                    </section>

                    <section className="settings-section">
                      <div>
                        <h3>Trash</h3>
                        <p>How long deleted files stay recoverable before cleanup.</p>
                      </div>
                      <div className="settings-form-grid">
                        <label>
                          Trash Retention Days
                          <input
                            min="1"
                            required
                            type="number"
                            value={systemSettingsForm.trashRetentionInDays}
                            onChange={(event) => setSystemSettingsForm((current) => ({
                              ...current,
                              trashRetentionInDays: event.target.value
                            }))}
                          />
                        </label>
                      </div>
                    </section>

                    <div className="settings-form-footer">
                      <button className="primary-button" disabled={savingSystemSettings || allowedExtensionList.length === 0} type="submit">
                        {savingSystemSettings ? "Saving..." : "Save Settings"}
                      </button>
                      {systemSettings && (
                        <span>Last updated {formatDate(getValue(systemSettings, "updatedAt", "UpdatedAt"))}</span>
                      )}
                    </div>
                  </form>
                )}
              </section>
            ) : activeModule && (
              <section className="panel module-detail">
                <div>
                  <p className="eyebrow">{activeModule.area}</p>
                  <h2>{activeModule.title}</h2>
                </div>
                <p>{activeModule.description}</p>
                <dl>
                  <div>
                    <dt>Required permission</dt>
                    <dd>{activeModule.permissions.join(" or ")}</dd>
                  </div>
                  <div>
                    <dt>Main API route</dt>
                    <dd>{activeModule.route}</dd>
                  </div>
                  {activeModule.note && (
                    <div>
                      <dt>Backend note</dt>
                      <dd>{activeModule.note}</dd>
                    </div>
                  )}
                </dl>
              </section>
            )}
          </section>

          <aside className="side-panels">
            <section className="panel dashboard-side-card">
              <h2>Infrastructure Snapshot</h2>
              <dl className="dashboard-side-list">
                <div>
                  <dt>Healthy nodes</dt>
                  <dd>{healthyNodesCount}</dd>
                </div>
                <div>
                  <dt>Total nodes</dt>
                  <dd>{storageNodes.length}</dd>
                </div>
                <div>
                  <dt>Count status</dt>
                  <dd>{loadingCounts ? "Refreshing" : "Ready"}</dd>
                </div>
              </dl>
              {nodeUsagePairs.length > 0 && (
                <div className="dashboard-mini-list compact">
                  {nodeUsagePairs.map((node) => (
                    <article className="dashboard-mini-row" key={node.id}>
                      <div>
                        <strong>{node.name}</strong>
                        <span>Usage</span>
                      </div>
                      <span>{node.percent.toFixed(0)}%</span>
                    </article>
                  ))}
                </div>
              )}
            </section>

            <section className="panel dashboard-side-card">
              <h2>Billing Snapshot</h2>
              <dl className="dashboard-side-list">
                <div>
                  <dt>Visible modules</dt>
                  <dd>{visibleModules.length}</dd>
                </div>
                <div>
                  <dt>Subscriptions</dt>
                  <dd>{subscriptions.length}</dd>
                </div>
                <div>
                  <dt>Permission entries</dt>
                  <dd>{permissions.length}</dd>
                </div>
              </dl>
              {recentSubscriptions.length > 0 && (
                <div className="dashboard-mini-list compact">
                  {recentSubscriptions.map((subscription) => (
                    <article className="dashboard-mini-row" key={getValue(subscription, "subscriptionId", "SubscriptionId")}>
                      <div>
                        <strong>{getValue(subscription, "planName", "PlanName") || "Subscription"}</strong>
                        <span>{getUserIdsText(subscription, usersById)}</span>
                      </div>
                      <span>{getSubscriptionStatusLabel(subscription)}</span>
                    </article>
                  ))}
                </div>
              )}
            </section>
          </aside>
        </section>
      </section>
    </main>
  );
}

function getAdminModuleIcon(key) {
  switch (key) {
    case "dashboard":
      return LayoutDashboard;
    case "users":
      return Users;
    case "storage":
      return Server;
    case "settings":
      return Settings;
    case "purchases":
      return ShoppingCart;
    case "subscriptions":
      return CreditCard;
    default:
      return Shield;
  }
}

function formatCount(countInfo, isLoading) {
  if (!countInfo && isLoading) return "Loading";
  if (!countInfo) return "Ready";
  if (countInfo.status === "error") return "Not available";
  return String(countInfo.value);
}

function cleanDisplayName(value) {
  const text = String(value ?? "").trim();
  if (!text || text.toLowerCase() === "undefined" || text.toLowerCase() === "null") {
    return "";
  }

  return text;
}

function getValue(source, ...keys) {
  for (const key of keys) {
    const value = source?.[key];
    if (value !== undefined && value !== null && String(value).trim()) {
      return String(value).trim();
    }
  }

  return "";
}

function getBooleanValue(source, ...keys) {
  for (const key of keys) {
    const value = source?.[key];
    if (value !== undefined && value !== null) {
      return value === true || String(value).toLowerCase() === "true";
    }
  }

  return false;
}

function getRegistrationModeValue(settings) {
  const value = getValue(settings, "registrationMode", "RegistrationMode");
  const matchingOption = registrationModeOptions.find((option) =>
    option.value === value || option.label.toLowerCase().replace(/\s/g, "") === value.toLowerCase().replace(/\s/g, "")
  );

  return matchingOption?.value ?? "1";
}

function mapSystemSettingsToForm(settings) {
  return {
    maxFileSizeInBytes: getValue(settings, "maxFileSizeInBytes", "MaxFileSizeInBytes"),
    defaultUserStorageQuotaInBytes: getValue(settings, "defaultUserStorageQuotaInBytes", "DefaultUserStorageQuotaInBytes"),
    allowedFileExtensions: getValue(settings, "allowedFileExtensions", "AllowedFileExtensions"),
    allowPublicLinkSharing: getBooleanValue(settings, "allowPublicLinkSharing", "AllowPublicLinkSharing"),
    defaultLinkExpirationInDays: getValue(settings, "defaultLinkExpirationInDays", "DefaultLinkExpirationInDays"),
    enforceLinkPasswordProtection: getBooleanValue(settings, "enforceLinkPasswordProtection", "EnforceLinkPasswordProtection"),
    registrationMode: getRegistrationModeValue(settings),
    twoFactorAuthRequired: getBooleanValue(settings, "twoFactorAuthRequired", "TwoFactorAuthRequired"),
    maxLoginAttempts: getValue(settings, "maxLoginAttempts", "MaxLoginAttempts"),
    minimumPasswordLength: getValue(settings, "minimumPasswordLength", "MinimumPasswordLength"),
    requireUppercasePassword: getBooleanValue(settings, "requireUppercasePassword", "RequireUppercasePassword"),
    requireNumberPassword: getBooleanValue(settings, "requireNumberPassword", "RequireNumberPassword"),
    requireSpecialCharacterPassword: getBooleanValue(settings, "requireSpecialCharacterPassword", "RequireSpecialCharacterPassword"),
    trashRetentionInDays: getValue(settings, "trashRetentionInDays", "TrashRetentionInDays")
  };
}

function parseAllowedFileExtensions(value) {
  return normalizeExtensionInputs(value);
}

function normalizeExtensionInputs(value) {
  return Array.from(
    new Set(
      String(value ?? "")
        .split(/[\s,;]+/)
        .map(normalizeExtension)
        .filter(Boolean)
    )
  );
}

function normalizeExtension(value) {
  let text = String(value ?? "").trim().toLowerCase();
  if (!text) return "";

  text = text.replace(/^\*+/, "");
  if (!text.startsWith(".")) {
    text = `.${text}`;
  }

  text = text.replace(/[^.a-z0-9_-]/g, "");
  return text.length > 1 ? text : "";
}

function toNullableNumber(value) {
  if (value === "" || value === null || value === undefined) {
    return null;
  }

  return Number(value);
}

function getUserDisplayName(user) {
  const firstName = getValue(user, "firstName", "FirstName");
  const lastName = getValue(user, "lastName", "LastName");
  const fullName = [firstName, lastName].filter(Boolean).join(" ");
  return fullName || getValue(user, "username", "Username") || getValue(user, "email", "Email") || "User";
}

function getNodeStatusValue(node) {
  const status = getValue(node, "status", "Status");
  const matchingOption = nodeStatusOptions.find((option) =>
    option.value === status || option.label.toLowerCase() === status.toLowerCase()
  );

  return matchingOption?.value ?? "1";
}

function getNodeStatusLabel(node) {
  const status = getNodeStatusValue(node);
  return nodeStatusOptions.find((option) => option.value === status)?.label ?? "Offline";
}

function getPurchaseStatusValue(purchase) {
  const status = getValue(purchase, "status", "Status");
  const matchingOption = purchaseStatusOptions.find((option) =>
    option.value === status || option.label.toLowerCase() === status.toLowerCase()
  );

  return matchingOption?.value ?? "0";
}

function getPurchaseStatusLabel(purchase) {
  const status = getPurchaseStatusValue(purchase);
  return purchaseStatusOptions.find((option) => option.value === status)?.label ?? "Pending";
}

function getBillingIntervalValue(plan) {
  const interval = getValue(plan, "billingInterval", "BillingInterval");
  const matchingOption = billingIntervalOptions.find((option) =>
    option.value === interval || option.label.toLowerCase() === interval.toLowerCase()
  );

  return matchingOption?.value ?? "0";
}

function getBillingIntervalLabel(plan) {
  const interval = getBillingIntervalValue(plan);
  return billingIntervalOptions.find((option) => option.value === interval)?.label ?? "Monthly";
}

function getSubscriptionStatusValue(subscription) {
  const status = getValue(subscription, "status", "Status");
  const matchingOption = subscriptionStatusOptions.find((option) =>
    option.value === status || option.label.toLowerCase().replace(/\s/g, "") === status.toLowerCase().replace(/\s/g, "")
  );

  return matchingOption?.value ?? "0";
}

function getSubscriptionStatusLabel(subscription) {
  const status = getSubscriptionStatusValue(subscription);
  return subscriptionStatusOptions.find((option) => option.value === status)?.label ?? "Pending";
}

function mapSubscriptionPlanToForm(plan) {
  return {
    subscriptionPlanId: getValue(plan, "subscriptionPlanId", "SubscriptionPlanId"),
    name: getValue(plan, "name", "Name"),
    description: getValue(plan, "description", "Description"),
    price: getValue(plan, "price", "Price"),
    currency: getValue(plan, "currency", "Currency") || "EUR",
    billingInterval: getBillingIntervalValue(plan),
    storageLimitBytes: getValue(plan, "storageLimitBytes", "StorageLimitBytes"),
    maxFileSizeBytes: getValue(plan, "maxFileSizeBytes", "MaxFileSizeBytes"),
    isActive: getBooleanValue(plan, "isActive", "IsActive")
  };
}

function getUserLookupLabel(userId, usersById) {
  const user = usersById[userId];
  if (!user) {
    return userId || "Unknown user";
  }

  return getUserDisplayName(user);
}

function getUserIdsText(subscription, usersById) {
  const userIds = subscription?.userIds ?? subscription?.UserIds ?? [];
  if (!Array.isArray(userIds) || userIds.length === 0) {
    return "No users";
  }

  return userIds
    .map((userId) => getUserLookupLabel(userId, usersById))
    .join(", ");
}

function resolveUserIdFromSearch(value, userDirectory) {
  const rawValue = String(value ?? "").trim();
  const query = rawValue.toLowerCase();
  if (!query) {
    return "";
  }

  const directMatch = userDirectory.find((user) => {
    const userId = getValue(user, "userId", "UserId").toLowerCase();
    const username = getValue(user, "username", "Username").toLowerCase();
    const email = getValue(user, "email", "Email").toLowerCase();
    const fullName = `${getValue(user, "firstName", "FirstName")} ${getValue(user, "lastName", "LastName")}`.trim().toLowerCase();

    return userId === query || username === query || email === query || fullName === query;
  });

  if (directMatch) {
    return getValue(directMatch, "userId", "UserId");
  }

  return /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(rawValue)
    ? rawValue
    : "";
}

function formatBytes(bytes) {
  if (!bytes) return "0 B";

  const units = ["B", "KB", "MB", "GB", "TB"];
  const index = Math.min(Math.floor(Math.log(bytes) / Math.log(1024)), units.length - 1);
  const value = bytes / Math.pow(1024, index);

  return `${value.toFixed(value >= 10 || index === 0 ? 0 : 1)} ${units[index]}`;
}

function formatMoney(amount, currency) {
  const value = Number(amount);
  const safeCurrency = currency || "EUR";

  if (Number.isNaN(value)) {
    return `${amount || "0"} ${safeCurrency}`;
  }

  return `${value.toFixed(2)} ${safeCurrency}`;
}

function formatDate(value) {
  if (!value) return "Not set";

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;

  return date.toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric"
  });
}

export default AdminDashboardPage;
