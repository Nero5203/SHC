import { useState, useEffect, useCallback, useRef } from "react";
import {
  getJson,
  postJson,
  putJson,
  deleteJson,
  uploadFile,
  downloadFileBlob,
  defaultApiUrl,
  getUserIdFromToken,
  getUserNameFromToken,
  getPermissionsFromToken,
  getRolesFromToken,
  clearAuth,
  isAuthenticated
} from "./apiClient.js";

const userDashboardModules = [
  { title: "My Files", area: "Storage", permissions: ["File.Read"], fallbackUser: true },
  { title: "Upload Files", area: "Storage", permissions: ["File.Upload"] },
  { title: "Folders", area: "Storage", permissions: ["Folder.Create"] },
  { title: "Link Sharing", area: "Sharing", permissions: ["File.Share", "Folder.Share"] },
  { title: "Notifications", area: "Activity", permissions: [], fallbackUser: true },
  { title: "Subscription", area: "Billing", permissions: [], fallbackUser: true }
];

const sharePermissionOptions = [
  { value: "0", label: "View" },
  { value: "1", label: "Download" },
  { value: "2", label: "Edit" }
];

const emptyShareForm = {
  permission: "0"
};

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

function formatBytes(bytes) {
  if (bytes === 0) return "0 B";
  const k = 1024;
  const sizes = ["B", "KB", "MB", "GB", "TB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(2))} ${sizes[i]}`;
}

function formatDate(dateString) {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleDateString("en-US", {
    month: "short",
    day: "numeric",
    year: "numeric"
  });
}

function formatMoney(amount, currency) {
  const value = Number(amount);
  const safeCurrency = currency || "EUR";

  if (Number.isNaN(value)) {
    return `${amount ?? 0} ${safeCurrency}`;
  }

  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: safeCurrency
  }).format(value);
}

function getProfileValue(profile, ...keys) {
  if (!profile) return "";

  for (const key of keys) {
    const value = profile[key];
    if (value !== undefined && value !== null && String(value).trim()) {
      return String(value).trim();
    }
  }

  return "";
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

function getArrayValue(source, ...keys) {
  for (const key of keys) {
    const value = source?.[key];
    if (Array.isArray(value)) {
      return value;
    }
  }

  return [];
}

function normalizeFolderContents(data) {
  return {
    folders: getArrayValue(data, "folders", "Folders"),
    files: getArrayValue(data, "files", "Files")
  };
}

function getFileId(file) {
  return getValue(file, "fileItemId", "FileItemId");
}

function getFileName(file) {
  return getValue(file, "fileName", "FileName") || "Unnamed file";
}

function getFolderId(folder) {
  return getValue(folder, "folderId", "FolderId");
}

function getFolderName(folder) {
  return getValue(folder, "name", "Name") || "Unnamed folder";
}

function getSharedLinkId(link) {
  return getValue(link, "sharedLinkId", "SharedLinkId");
}

function getShareUrl(apiUrl, link) {
  const directShareUrl = getValue(link, "shareUrl", "ShareUrl");
  if (directShareUrl) {
    return `${apiUrl.replace(/\/$/, "")}${directShareUrl}`;
  }

  const tokenUrl = getValue(link, "tokenUrl", "TokenUrl");
  if (!tokenUrl) return "";

  return `${apiUrl.replace(/\/$/, "")}/api/shared-links/token/${tokenUrl}`;
}

function getShareTargetTypeLabel(link) {
  const targetType = getValue(link, "targetType", "TargetType");
  if (targetType === "0" || targetType.toLowerCase() === "file") return "File";
  if (targetType === "1" || targetType.toLowerCase() === "folder") return "Folder";
  return "Item";
}

function getSharePermissionLabel(link) {
  const canEdit = getBooleanValue(link, "canEdit", "CanEdit");
  const allowDownload = getBooleanValue(link, "allowDownload", "AllowDownload");

  if (canEdit) return "Edit";
  if (allowDownload) return "Download";
  return "View";
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

function getProfileDisplayName(profile, fallbackName) {
  const firstName = getProfileValue(profile, "firstName", "FirstName");
  const lastName = getProfileValue(profile, "lastName", "LastName");
  const fullName = [firstName, lastName].filter(Boolean).join(" ");
  const username = getProfileValue(profile, "username", "Username");
  const email = getProfileValue(profile, "email", "Email");

  return fullName || username || email || fallbackName;
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

function getSubscriptionStatusValue(subscriptionItem) {
  const status = getValue(subscriptionItem, "status", "Status");
  const matchingOption = subscriptionStatusOptions.find((option) =>
    option.value === status || option.label.toLowerCase().replace(/\s/g, "") === status.toLowerCase().replace(/\s/g, "")
  );

  return matchingOption?.value ?? "5";
}

function getSubscriptionStatusLabel(subscriptionItem) {
  const status = getSubscriptionStatusValue(subscriptionItem);
  return subscriptionStatusOptions.find((option) => option.value === status)?.label ?? "Pending";
}

function getNotificationId(notification) {
  return getValue(notification, "notificationId", "NotificationId");
}

function getNotificationTypeValue(notification) {
  return getValue(notification, "type", "Type") || "5";
}

function getNotificationTypeLabel(notification) {
  switch (String(getNotificationTypeValue(notification)).toLowerCase()) {
    case "1":
    case "fileshared":
      return "File Shared";
    case "2":
    case "foldershared":
      return "Folder Shared";
    case "3":
    case "fileupdated":
      return "File Updated";
    case "4":
    case "storagewarning":
      return "Storage Warning";
    case "5":
    case "systemalert":
      return "System Alert";
    default:
      return "Notification";
  }
}

function getNotificationIconLabel(notification) {
  switch (String(getNotificationTypeValue(notification)).toLowerCase()) {
    case "1":
    case "2":
      return "SH";
    case "3":
      return "UP";
    case "4":
      return "ST";
    case "5":
      return "SY";
    default:
      return "NT";
  }
}

function UserHomePage({ onLogout }) {
  const [apiUrl, setApiUrl] = useState(() => localStorage.getItem("shc.apiUrl") || defaultApiUrl);
  const [activeTab, setActiveTab] = useState("files");

  const [profile, setProfile] = useState(null);
  const [loadingProfile, setLoadingProfile] = useState(true);

  const [subscription, setSubscription] = useState(null);
  const [userSubscriptions, setUserSubscriptions] = useState([]);
  const [subscriptionPlans, setSubscriptionPlans] = useState([]);
  const [entitlements, setEntitlements] = useState(null);
  const [loadingSubscription, setLoadingSubscription] = useState(true);
  const [subscriptionActionKey, setSubscriptionActionKey] = useState("");
  const [subscriptionError, setSubscriptionError] = useState("");
  const [checkoutMessage, setCheckoutMessage] = useState("");

  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [loadingNotifications, setLoadingNotifications] = useState(true);
  const [notificationFilter, setNotificationFilter] = useState("all");
  const [notificationActionKey, setNotificationActionKey] = useState("");
  const [sharedLinks, setSharedLinks] = useState([]);
  const [loadingSharedLinks, setLoadingSharedLinks] = useState(true);
  const [shareActionKey, setShareActionKey] = useState("");
  const [shareForm, setShareForm] = useState(emptyShareForm);
  const [shareTarget, setShareTarget] = useState(null);
  const [shareResult, setShareResult] = useState(null);
  const [editingSharedLinkId, setEditingSharedLinkId] = useState(null);
  const [editingSharedLinkForm, setEditingSharedLinkForm] = useState({
    expirationDate: null,
    canView: true,
    canEdit: false,
    allowDownload: true
  });

  const [folderContents, setFolderContents] = useState({ folders: [], files: [] });
  const [currentFolder, setCurrentFolder] = useState(null);
  const [loadingFiles, setLoadingFiles] = useState(true);

  const [newFolderName, setNewFolderName] = useState("");
  const [creatingFolder, setCreatingFolder] = useState(false);

  const [uploading, setUploading] = useState(false);
  const [uploadError, setUploadError] = useState("");

  const [selectedFile, setSelectedFile] = useState(null);
  const [downloadProgress, setDownloadProgress] = useState(null);

  const [deletingId, setDeletingId] = useState(null);
  const [renamingId, setRenamingId] = useState(null);
  const [renameValue, setRenameValue] = useState("");
  const [moveTargetByFile, setMoveTargetByFile] = useState({});
  const [movingId, setMovingId] = useState(null);

  const fileInputRef = useRef(null);

  const userId = getUserIdFromToken();
  const tokenUserName = getUserNameFromToken() || "User";
  const userDisplayName = getProfileDisplayName(profile, tokenUserName);
  const userEmail = getProfileValue(profile, "email", "Email");
  const roles = getRolesFromToken();
  const permissions = getPermissionsFromToken();
  const permissionSet = new Set(permissions.map((permission) => permission.toLowerCase()));
  const hasUserRole = roles.some((role) => role.toLowerCase() === "user" || role.toLowerCase() === "admin");
  const authorizedModules = userDashboardModules.filter((module) =>
    module.permissions.some((permission) => permissionSet.has(permission.toLowerCase()))
    || (module.fallbackUser && hasUserRole)
  );

  function updateApiUrl(value) {
    setApiUrl(value);
    localStorage.setItem("shc.apiUrl", value);
  }

  const fetchProfile = useCallback(async () => {
    if (!userId) return;
    try {
      const user = await getJson(apiUrl, `/api/users/${userId}`);
      setProfile(user);
    } catch (error) {
      console.error("Failed to fetch profile:", error);
    } finally {
      setLoadingProfile(false);
    }
  }, [apiUrl, userId]);

  const fetchSubscriptionData = useCallback(async () => {
    if (!userId) return;
    try {
      const [activeSub, userEntitlements, plans, subscriptions] = await Promise.all([
        getJson(apiUrl, `/api/subscriptions/user/${userId}/active`).catch(() => null),
        getJson(apiUrl, `/api/subscriptions/user/${userId}/entitlements`).catch(() => null),
        getJson(apiUrl, "/api/subscriptions/plans?activeOnly=true").catch(() => []),
        getJson(apiUrl, `/api/subscriptions/user/${userId}`).catch(() => [])
      ]);
      setSubscription(activeSub);
      setEntitlements(userEntitlements);
      setSubscriptionPlans(Array.isArray(plans) ? plans : []);
      setUserSubscriptions(Array.isArray(subscriptions) ? subscriptions : []);
    } catch (error) {
      console.error("Failed to fetch subscription:", error);
    } finally {
      setLoadingSubscription(false);
    }
  }, [apiUrl, userId]);

  const fetchNotifications = useCallback(async () => {
    if (!userId) return;
    try {
      const items = await getJson(apiUrl, `/api/notifications/user/${userId}`);
      const notificationItems = Array.isArray(items) ? items : [];
      setNotifications(notificationItems);
      setUnreadCount(notificationItems.filter((item) => !getBooleanValue(item, "isRead", "IsRead")).length);
    } catch (error) {
      console.error("Failed to fetch notifications:", error);
    } finally {
      setLoadingNotifications(false);
    }
  }, [apiUrl, userId]);

  const fetchSharedLinks = useCallback(async () => {
    if (!userId) return;
    try {
      const links = await getJson(apiUrl, `/api/shared-links/user/${userId}`);
      setSharedLinks(Array.isArray(links) ? links : []);
    } catch (error) {
      console.error("Failed to fetch shared links:", error);
    } finally {
      setLoadingSharedLinks(false);
    }
  }, [apiUrl, userId]);

  const fetchFolderContents = useCallback(async (folderId = null) => {
    if (!userId) return;
    setLoadingFiles(true);
    try {
      const url = folderId
        ? `/api/folders/user/${userId}/contents?folderId=${folderId}`
        : `/api/folders/user/${userId}/contents`;
      const data = await getJson(apiUrl, url);
      setFolderContents(normalizeFolderContents(data));
      setCurrentFolder(folderId);
    } catch (error) {
      console.error("Failed to fetch folder contents:", error);
    } finally {
      setLoadingFiles(false);
    }
  }, [apiUrl, userId]);

  useEffect(() => {
    if (!isAuthenticated()) {
      onLogout("login");
      return;
    }
    fetchProfile();
    fetchSubscriptionData();
    fetchNotifications();
    fetchSharedLinks();
    fetchFolderContents();
  }, [onLogout, fetchProfile, fetchSubscriptionData, fetchNotifications, fetchSharedLinks, fetchFolderContents]);

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const checkoutStatus = params.get("checkout");

    if (!checkoutStatus) {
      return;
    }

    if (checkoutStatus === "success") {
      setCheckoutMessage("Payment completed. Your subscription will update after Stripe confirms the payment.");
      fetchSubscriptionData();
      fetchNotifications();
    } else if (checkoutStatus === "cancelled") {
      setCheckoutMessage("Checkout was cancelled. Your subscription remains pending until payment is completed.");
      fetchSubscriptionData();
      fetchNotifications();
    }

    params.delete("checkout");
    params.delete("purchaseId");
    const nextQuery = params.toString();
    const nextUrl = `${window.location.pathname}${nextQuery ? `?${nextQuery}` : ""}`;
    window.history.replaceState({}, "", nextUrl);
  }, [fetchNotifications, fetchSubscriptionData]);

  const handleMarkNotificationAsRead = async (notificationId) => {
    if (!notificationId) return;

    setNotificationActionKey(`${notificationId}:read`);
    try {
      await putJson(apiUrl, `/api/notifications/${notificationId}/read`, {});
      await fetchNotifications();
    } catch (error) {
      alert(`Failed to mark notification as read: ${error.message}`);
    } finally {
      setNotificationActionKey("");
    }
  };

  const handleMarkAllNotificationsAsRead = async () => {
    if (!userId || unreadCount === 0) return;

    setNotificationActionKey("all:read");
    try {
      await putJson(apiUrl, `/api/notifications/user/${userId}/read-all`, {});
      await fetchNotifications();
    } catch (error) {
      alert(`Failed to mark all notifications as read: ${error.message}`);
    } finally {
      setNotificationActionKey("");
    }
  };

  const handleDeleteNotification = async (notification) => {
    const notificationId = getNotificationId(notification);
    if (!notificationId) return;

    setNotificationActionKey(`${notificationId}:delete`);
    try {
      await deleteJson(apiUrl, `/api/notifications/${notificationId}`);
      await fetchNotifications();
    } catch (error) {
      alert(`Failed to delete notification: ${error.message}`);
    } finally {
      setNotificationActionKey("");
    }
  };

  const handleCreateFolder = async (event) => {
    event.preventDefault();
    if (!newFolderName.trim() || !userId) return;

    setCreatingFolder(true);
    try {
      await postJson(apiUrl, "/api/folders", {
        UserId: userId,
        Name: newFolderName.trim(),
        ParentFolderId: currentFolder
      });
      setNewFolderName("");
      fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to create folder: ${error.message}`);
    } finally {
      setCreatingFolder(false);
    }
  };

  const handleUploadClick = () => {
    fileInputRef.current?.click();
  };

  const handleFileChange = async (event) => {
    const file = event.target.files?.[0];
    if (!file || !userId) return;

    setSelectedFile(file);
    setUploadError("");
    setUploading(true);

    try {
      await uploadFile(apiUrl, userId, currentFolder, file);
      await fetchFolderContents(currentFolder);
      setSelectedFile(null);
    } catch (error) {
      setUploadError(error.message);
    } finally {
      setUploading(false);
      event.target.value = "";
    }
  };

  const handleDownload = async (fileItem) => {
    const fileItemId = getFileId(fileItem);
    const fileName = getFileName(fileItem);

    if (!fileItemId) {
      alert("File id was not found.");
      return;
    }

    setDownloadProgress(fileItemId);
    try {
      const blob = await downloadFileBlob(apiUrl, fileItemId);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      alert(`Failed to download file: ${error.message}`);
    } finally {
      setDownloadProgress(null);
    }
  };

  const handleDeleteFile = async (fileItem) => {
    const fileItemId = getFileId(fileItem);
    const fileName = getFileName(fileItem);

    if (!fileItemId) {
      alert("File id was not found.");
      return;
    }

    if (!window.confirm(`Delete "${fileName}"?`)) return;
    setDeletingId(fileItemId);
    try {
      await deleteJson(apiUrl, `/api/files/${fileItemId}`);
      await fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to delete file: ${error.message}`);
    } finally {
      setDeletingId(null);
    }
  };

  const handleDeleteFolder = async (folder) => {
    const folderId = getFolderId(folder);
    const folderName = getFolderName(folder);

    if (!folderId) {
      alert("Folder id was not found.");
      return;
    }

    if (!window.confirm(`Delete folder "${folderName}" and all its contents?`)) return;
    setDeletingId(folderId);
    try {
      await deleteJson(apiUrl, `/api/folders/${folderId}`);
      await fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to delete folder: ${error.message}`);
    } finally {
      setDeletingId(null);
    }
  };

  const startRename = (item) => {
    const itemId = getFileId(item) || getFolderId(item);
    const itemName = getFileId(item) ? getFileName(item) : getFolderName(item);

    setRenamingId(itemId);
    setRenameValue(itemName);
  };

  const cancelRename = () => {
    setRenamingId(null);
    setRenameValue("");
  };

  const submitRename = async (item) => {
    const trimmed = renameValue.trim();
    if (!trimmed) {
      cancelRename();
      return;
    }

    const isFile = !!getFileId(item);
    const id = isFile ? getFileId(item) : getFolderId(item);
    const endpoint = isFile
      ? `/api/files/${id}/rename`
      : `/api/folders/${id}/rename`;

    try {
      const body = isFile
        ? { FileItemId: id, NewName: trimmed }
        : { FolderId: id, NewName: trimmed };

      await putJson(apiUrl, endpoint, body);

      await fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to rename: ${error.message}`);
    } finally {
      setRenamingId(null);
      setRenameValue("");
    }
  };

  const handleMoveFile = async (fileItem) => {
    const fileItemId = getFileId(fileItem);
    const targetFolderId = moveTargetByFile[fileItemId];

    if (!fileItemId || !targetFolderId) return;

    setMovingId(fileItemId);
    try {
      await putJson(apiUrl, `/api/files/${fileItemId}/move`, {
        FileItemId: fileItemId,
        TargetFolderId: targetFolderId
      });
      setMoveTargetByFile((current) => ({ ...current, [fileItemId]: "" }));
      await fetchFolderContents(currentFolder);
    } catch (error) {
      alert(`Failed to move file: ${error.message}`);
    } finally {
      setMovingId(null);
    }
  };

  const openShareDialog = (targetType, item) => {
    setShareTarget({
      targetType,
      itemId: targetType === "file" ? getFileId(item) : getFolderId(item),
      name: targetType === "file" ? getFileName(item) : getFolderName(item)
    });
    setShareForm(emptyShareForm);
    setShareResult(null);
  };

  const closeShareDialog = () => {
    setShareTarget(null);
    setShareForm(emptyShareForm);
  };

  const handleCreateShareLink = async (event) => {
    event.preventDefault();
    if (!shareTarget || !userId) return;

    const actionKey = `${shareTarget.targetType}:${shareTarget.itemId}:share`;
    setShareActionKey(actionKey);

    try {
      const endpoint = shareTarget.targetType === "file"
        ? `/api/files/${shareTarget.itemId}/share`
        : `/api/folders/${shareTarget.itemId}/share`;

      const body = shareTarget.targetType === "file"
        ? {
            FileItemId: shareTarget.itemId,
            Permission: Number(shareForm.permission)
          }
        : {
            FolderId: shareTarget.itemId,
            Permission: Number(shareForm.permission)
          };

      const response = await postJson(apiUrl, endpoint, body);
      setShareResult(response);
      await fetchSharedLinks();
    } catch (error) {
      alert(`Failed to create share link: ${error.message}`);
    } finally {
      setShareActionKey("");
    }
  };

  const startEditSharedLink = (link) => {
    setEditingSharedLinkId(getSharedLinkId(link));
    setEditingSharedLinkForm({
      expirationDate: getValue(link, "expirationDate", "ExpirationDate") || null,
      canView: getBooleanValue(link, "canView", "CanView"),
      canEdit: getBooleanValue(link, "canEdit", "CanEdit"),
      allowDownload: getBooleanValue(link, "allowDownload", "AllowDownload")
    });
  };

  const cancelEditSharedLink = () => {
    setEditingSharedLinkId(null);
  };

  const handleUpdateSharedLink = async (sharedLinkId) => {
    setShareActionKey(`${sharedLinkId}:update`);

    try {
      await putJson(apiUrl, `/api/shared-links/${sharedLinkId}`, {
        ExpirationDate: editingSharedLinkForm.expirationDate,
        CanView: editingSharedLinkForm.canView,
        CanEdit: editingSharedLinkForm.canEdit,
        AllowDownload: editingSharedLinkForm.allowDownload
      });

      setEditingSharedLinkId(null);
      await fetchSharedLinks();
    } catch (error) {
      alert(`Failed to update share link: ${error.message}`);
    } finally {
      setShareActionKey("");
    }
  };

  const handleDeactivateSharedLink = async (sharedLinkId) => {
    setShareActionKey(`${sharedLinkId}:deactivate`);

    try {
      await putJson(apiUrl, `/api/shared-links/${sharedLinkId}/deactivate`, {});
      await fetchSharedLinks();
    } catch (error) {
      alert(`Failed to deactivate share link: ${error.message}`);
    } finally {
      setShareActionKey("");
    }
  };

  const handleCopyShareUrl = async (link) => {
    const shareUrl = getShareUrl(apiUrl, link);
    if (!shareUrl) return;

    try {
      await navigator.clipboard.writeText(shareUrl);
    } catch (error) {
      window.prompt("Copy this share URL:", shareUrl);
    }
  };

  const handleLogout = async () => {
    try {
      const refreshToken = localStorage.getItem("shc.refreshToken");
      if (refreshToken) {
        await postJson(apiUrl, "/api/auth/logout", { RefreshToken: refreshToken }).catch(() => {});
      }
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      clearAuth();
      onLogout("login");
    }
  };

  const openFolder = (folderId) => {
    fetchFolderContents(folderId);
  };

  const goToParentFolder = () => {
    if (currentFolder) {
      fetchFolderContents(null);
    }
  };

  const latestSubscription = subscription || userSubscriptions[0] || null;
  const visibleNotifications = notifications.filter((notification) => {
    const isRead = getBooleanValue(notification, "isRead", "IsRead");

    if (notificationFilter === "unread") {
      return !isRead;
    }

    if (notificationFilter === "read") {
      return isRead;
    }

    return true;
  });

  const handleSubscribeToPlan = async (plan) => {
    if (!userId) {
      alert("User was not found in the current session.");
      return;
    }

    const planId = getValue(plan, "subscriptionPlanId", "SubscriptionPlanId");
    if (!planId) {
      alert("Subscription plan id was not found.");
      return;
    }

    setSubscriptionError("");
    setCheckoutMessage("");
    setSubscriptionActionKey(planId);

    try {
      const createdSubscription = await postJson(apiUrl, "/api/subscriptions", {
        UserId: userId,
        SubscriptionPlanId: planId,
        TrialEndsAt: null,
        AutoRenew: true,
        ProviderSubscriptionId: null
      });

      const subscriptionId = getValue(createdSubscription, "subscriptionId", "SubscriptionId");

      if (!subscriptionId) {
        throw new Error("Subscription was created without an id.");
      }

      const createdPurchase = await postJson(apiUrl, "/api/purchases", {
        UserId: userId,
        SubscriptionId: subscriptionId
      });

      const purchaseId = getValue(createdPurchase, "purchaseId", "PurchaseId");

      if (!purchaseId) {
        throw new Error("Purchase was created without an id.");
      }

      const baseUrl = `${window.location.origin}${window.location.pathname}`;
      const checkout = await postJson(apiUrl, `/api/purchases/${purchaseId}/checkout`, {
        SuccessUrl: `${baseUrl}?checkout=success&purchaseId=${purchaseId}`,
        CancelUrl: `${baseUrl}?checkout=cancelled&purchaseId=${purchaseId}`
      });

      const checkoutUrl = getValue(checkout, "checkoutUrl", "CheckoutUrl");

      if (!checkoutUrl) {
        throw new Error("Stripe checkout URL was not returned.");
      }

      await fetchSubscriptionData();
      window.location.assign(checkoutUrl);
    } catch (error) {
      setSubscriptionError(error.message);
    } finally {
      setSubscriptionActionKey("");
    }
  };

  const storageUsed = entitlements?.StorageLimitBytes ? (entitlements.StorageLimitBytes * 0.3) : 0;
  const storageLimit = entitlements?.StorageLimitBytes || 0;
  const storagePercentage = storageLimit > 0 ? Math.min((storageUsed / storageLimit) * 100, 100) : 0;

  const [redirecting, setRedirecting] = useState(false);

  useEffect(() => {
    if (!isAuthenticated()) {
      setRedirecting(true);
      onLogout("login");
    }
  }, [onLogout]);

  if (redirecting) {
    return (
      <main className="app-shell">
        <aside className="sidebar">
          <div className="brand">
            <div className="brand-mark">S</div>
            <div>
              <strong>SHC</strong>
              <span>Cloud Console</span>
            </div>
          </div>
        </aside>
        <section className="page">
          <header className="topbar">
            <div>
              <p className="eyebrow">Loading</p>
              <h1>Please wait...</h1>
            </div>
          </header>
        </section>
      </main>
    );
  }

  return (
    <main className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-mark">S</div>
          <div>
            <strong>SHC</strong>
            <span>Cloud Console</span>
          </div>
        </div>

        <nav className="module-list" aria-label="Frontend modules">
          <button className={`module-item ${activeTab === "files" ? "active" : ""}`} onClick={() => { setActiveTab("files"); fetchFolderContents(currentFolder); }} type="button">
            <span>My Files</span>
            <small>Storage</small>
          </button>
          <button className={`module-item ${activeTab === "sharing" ? "active" : ""}`} onClick={() => { setActiveTab("sharing"); fetchSharedLinks(); }} type="button">
            <span>Link Sharing</span>
            <small>Links</small>
          </button>
          <button className={`module-item ${activeTab === "notifications" ? "active" : ""}`} onClick={() => setActiveTab("notifications")} type="button">
            <span>Notifications</span>
            <small>{unreadCount > 0 ? unreadCount : ""}</small>
          </button>
          <button className={`module-item ${activeTab === "subscription" ? "active" : ""}`} onClick={() => setActiveTab("subscription")} type="button">
            <span>Subscription</span>
            <small>Plan</small>
          </button>
        </nav>

        <div style={{ marginTop: "auto" }}>
          <button className="module-item" onClick={handleLogout} type="button">
            <span>Logout</span>
            <small>Exit</small>
          </button>
        </div>
      </aside>

      <section className="page">
        <header className="topbar">
          <div>
            <p className="eyebrow">Dashboard</p>
            <h1>Welcome, {userDisplayName}</h1>
          </div>
          <label className="api-field">
            API URL
            <input value={apiUrl} onChange={(event) => updateApiUrl(event.target.value)} />
          </label>
        </header>

        {activeTab === "files" && (
          <section className="content-grid">
            <section className="files-panel">
              <div className="panel">
                <div className="files-header">
                  <div>
                    <h2>My Files</h2>
                    <p>Browse and manage your files and folders.</p>
                  </div>
                  <div className="files-actions">
                    <form className="new-folder-form" onSubmit={handleCreateFolder}>
                      <input
                        type="text"
                        placeholder="New folder name"
                        value={newFolderName}
                        onChange={(e) => setNewFolderName(e.target.value)}
                        disabled={creatingFolder}
                      />
                      <button className="secondary-button" type="submit" disabled={creatingFolder || !newFolderName.trim()}>
                        {creatingFolder ? "Creating..." : "New Folder"}
                      </button>
                    </form>
                    <button className="primary-button" type="button" onClick={handleUploadClick} disabled={uploading}>
                      {uploading ? "Uploading..." : "Upload File"}
                    </button>
                    <input
                      ref={fileInputRef}
                      type="file"
                      style={{ display: "none" }}
                      onChange={handleFileChange}
                    />
                  </div>
                </div>

                {currentFolder && (
                  <button className="breadcrumb" onClick={goToParentFolder} type="button">
                    ← Back to root
                  </button>
                )}

                {selectedFile && (
                  <div className="upload-status">
                    Uploading {selectedFile.name} ({formatBytes(selectedFile.size)})...
                  </div>
                )}

                {uploadError && (
                  <div className="upload-status error">{uploadError}</div>
                )}

                {loadingFiles ? (
                  <p className="loading-text">Loading files...</p>
                ) : folderContents.folders.length === 0 && folderContents.files.length === 0 ? (
                  <p className="empty-text">No files or folders yet. Create a folder to get started.</p>
                ) : (
                  <div className="file-list">
                    {folderContents.folders.map((folder) => {
                      const folderId = getFolderId(folder);
                      const folderName = getFolderName(folder);
                      const isRenaming = renamingId === folderId;
                      const isDeleting = deletingId === folderId;

                      return (
                        <div key={folderId} className="file-item folder">
                          <div className="file-icon folder-icon">📁</div>
                          <div className="file-info">
                            {isRenaming ? (
                              <input
                                className="rename-input"
                                type="text"
                                value={renameValue}
                                onChange={(e) => setRenameValue(e.target.value)}
                                onBlur={() => submitRename(folder)}
                                onKeyDown={(e) => {
                                  if (e.key === "Enter") submitRename(folder);
                                  if (e.key === "Escape") cancelRename();
                                }}
                                autoFocus
                              />
                            ) : (
                              <strong onClick={() => openFolder(folderId)}>{folderName}</strong>
                            )}
                            <small>Folder • Created {formatDate(getValue(folder, "createdAt", "CreatedAt"))}</small>
                          </div>
                          <div className="file-actions">
                            <button
                              className="icon-button"
                              onClick={() => openFolder(folderId)}
                              title="Open"
                              type="button"
                              disabled={isRenaming}
                            >
                              Open
                            </button>
                            <button
                              className="icon-button"
                              onClick={() => startRename(folder)}
                              title="Rename"
                              type="button"
                              disabled={isRenaming || isDeleting}
                            >
                              Rename
                            </button>
                            <button
                              className="icon-button"
                              onClick={() => openShareDialog("folder", folder)}
                              title="Share"
                              type="button"
                              disabled={isRenaming || isDeleting}
                            >
                              Share
                            </button>
                            <button
                              className="icon-button danger"
                              onClick={() => handleDeleteFolder(folder)}
                              title="Delete"
                              type="button"
                              disabled={isRenaming || isDeleting}
                            >
                              {isDeleting ? "Deleting..." : "Delete"}
                            </button>
                          </div>
                        </div>
                      );
                    })}
                    {folderContents.files.map((file) => {
                      const fileItemId = getFileId(file);
                      const fileName = getFileName(file);
                      const fileType = getValue(file, "fileType", "FileType") || "Unknown";
                      const fileSize = Number(getValue(file, "fileSize", "FileSize")) || 0;
                      const isRenaming = renamingId === fileItemId;
                      const isDeleting = deletingId === fileItemId;
                      const isDownloading = downloadProgress === fileItemId;
                      const isMoving = movingId === fileItemId;
                      const moveTarget = moveTargetByFile[fileItemId] ?? "";
                      const targetFolders = folderContents.folders.filter((folder) => getFolderId(folder));

                      return (
                        <div key={fileItemId} className="file-item">
                          <div className="file-icon">📄</div>
                          <div className="file-info">
                            {isRenaming ? (
                              <input
                                className="rename-input"
                                type="text"
                                value={renameValue}
                                onChange={(e) => setRenameValue(e.target.value)}
                                onBlur={() => submitRename(file)}
                                onKeyDown={(e) => {
                                  if (e.key === "Enter") submitRename(file);
                                  if (e.key === "Escape") cancelRename();
                                }}
                                autoFocus
                              />
                            ) : (
                              <strong>{fileName}</strong>
                            )}
                            <small>{fileType} • {formatBytes(fileSize)}</small>
                          </div>
                          <div className="file-actions">
                            <button
                              className="icon-button"
                              onClick={() => handleDownload(file)}
                              title="Download"
                              type="button"
                              disabled={isDownloading || isRenaming || isDeleting}
                            >
                              {isDownloading ? "Saving..." : "Download"}
                            </button>
                            <button
                              className="icon-button"
                              onClick={() => startRename(file)}
                              title="Rename"
                              type="button"
                              disabled={isRenaming || isDeleting || isDownloading}
                            >
                              Rename
                            </button>
                            <button
                              className="icon-button"
                              onClick={() => openShareDialog("file", file)}
                              title="Share"
                              type="button"
                              disabled={isRenaming || isDeleting || isDownloading}
                            >
                              Share
                            </button>
                            {targetFolders.length > 0 && (
                              <div className="move-control">
                                <select
                                  value={moveTarget}
                                  onChange={(event) =>
                                    setMoveTargetByFile((current) => ({
                                      ...current,
                                      [fileItemId]: event.target.value
                                    }))
                                  }
                                  disabled={isRenaming || isDeleting || isDownloading || isMoving}
                                >
                                  <option value="">Move to...</option>
                                  {targetFolders.map((folder) => {
                                    const targetFolderId = getFolderId(folder);

                                    return (
                                      <option key={targetFolderId} value={targetFolderId}>
                                        {getFolderName(folder)}
                                      </option>
                                    );
                                  })}
                                </select>
                                <button
                                  className="icon-button"
                                  onClick={() => handleMoveFile(file)}
                                  type="button"
                                  disabled={!moveTarget || isRenaming || isDeleting || isDownloading || isMoving}
                                >
                                  {isMoving ? "Moving..." : "Move"}
                                </button>
                              </div>
                            )}
                            <button
                              className="icon-button danger"
                              onClick={() => handleDeleteFile(file)}
                              title="Delete"
                              type="button"
                              disabled={isRenaming || isDeleting || isDownloading}
                            >
                              {isDeleting ? "Deleting..." : "Delete"}
                            </button>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                )}
              </div>
            </section>

            <aside className="side-panels">
              <div className="panel profile-card">
                <h2>Profile</h2>
                {loadingProfile ? (
                  <p>Loading...</p>
                ) : profile ? (
                  <div className="profile-info">
                    <div className="profile-avatar">
                      {userDisplayName[0]?.toUpperCase() || "?"}
                    </div>
                    <div>
                      <strong>{userDisplayName}</strong>
                      <p>{userEmail}</p>
                    </div>
                  </div>
                ) : (
                  <p>Failed to load profile.</p>
                )}
              </div>

              <div className="panel storage-card">
                <h2>Storage</h2>
                {loadingSubscription ? (
                  <p>Loading...</p>
                ) : (
                  <div>
                    <div className="storage-bar">
                      <div className="storage-fill" style={{ width: `${storagePercentage}%` }}></div>
                    </div>
                    <p className="storage-text">
                      {formatBytes(storageUsed)} of {formatBytes(storageLimit)} used
                    </p>
                    {entitlements?.PlanName && (
                      <p className="plan-badge">{entitlements.PlanName}</p>
                    )}
                  </div>
                )}
              </div>

              <div className="panel access-card">
                <h2>Your Access</h2>
                <div className="pill-list compact">
                  {roles.length > 0 ? roles.map((role) => <span key={role}>{role}</span>) : <span>No role in token</span>}
                </div>
                <div className="permission-list compact">
                  {authorizedModules.length > 0
                    ? authorizedModules.map((module) => <span key={module.title}>{module.title}</span>)
                    : <span>No dashboard modules found</span>}
                </div>
              </div>
            </aside>
          </section>
        )}

        {activeTab === "notifications" && (
          <section className="content-grid">
            <section className="notifications-panel">
              <div className="panel">
                <div className="notifications-header">
                  <h2>Notifications</h2>
                  <div className="notifications-toolbar">
                    {unreadCount > 0 && (
                      <span className="unread-badge">{unreadCount} unread</span>
                    )}
                    <button
                      className="secondary-button"
                      onClick={fetchNotifications}
                      type="button"
                      disabled={loadingNotifications || notificationActionKey === "all:read"}
                    >
                      {loadingNotifications ? "Refreshing..." : "Refresh"}
                    </button>
                    <button
                      className="secondary-button"
                      onClick={handleMarkAllNotificationsAsRead}
                      type="button"
                      disabled={unreadCount === 0 || notificationActionKey === "all:read"}
                    >
                      {notificationActionKey === "all:read" ? "Saving..." : "Mark all read"}
                    </button>
                  </div>
                </div>
                {loadingNotifications ? (
                  <p className="loading-text">Loading notifications...</p>
                ) : (
                  <>
                    <div className="notification-filters">
                      <button
                        className={`secondary-button ${notificationFilter === "all" ? "is-active" : ""}`}
                        onClick={() => setNotificationFilter("all")}
                        type="button"
                      >
                        All
                      </button>
                      <button
                        className={`secondary-button ${notificationFilter === "unread" ? "is-active" : ""}`}
                        onClick={() => setNotificationFilter("unread")}
                        type="button"
                      >
                        Unread
                      </button>
                      <button
                        className={`secondary-button ${notificationFilter === "read" ? "is-active" : ""}`}
                        onClick={() => setNotificationFilter("read")}
                        type="button"
                      >
                        Read
                      </button>
                    </div>

                    {visibleNotifications.length === 0 ? (
                      <p className="empty-text">No notifications in this view.</p>
                    ) : (
                      <div className="notification-list">
                        {visibleNotifications.map((notification) => {
                          const notificationId = getNotificationId(notification);
                          const isRead = getBooleanValue(notification, "isRead", "IsRead");
                          const title = getValue(notification, "title", "Title") || "Notification";
                          const message = getValue(notification, "message", "Message");
                          const createdAt = getValue(notification, "createdAt", "CreatedAt");

                          return (
                            <div key={notificationId} className={`notification-item ${isRead ? "is-read" : "is-unread"}`}>
                              <div className="notification-icon">{getNotificationIconLabel(notification)}</div>
                              <div className="notification-content">
                                <div className="notification-top-row">
                                  <strong>{title}</strong>
                                  <span className={`notification-type-badge ${isRead ? "read" : "unread"}`}>
                                    {getNotificationTypeLabel(notification)}
                                  </span>
                                </div>
                                <p>{message}</p>
                                <div className="notification-meta-row">
                                  <small>{formatDate(createdAt)}</small>
                                  {!isRead && <small>Unread</small>}
                                </div>
                              </div>
                              <div className="notification-actions">
                                {!isRead && (
                                  <button
                                    className="secondary-button"
                                    onClick={() => handleMarkNotificationAsRead(notificationId)}
                                    type="button"
                                    disabled={notificationActionKey === `${notificationId}:read` || notificationActionKey === `${notificationId}:delete`}
                                  >
                                    {notificationActionKey === `${notificationId}:read` ? "Saving..." : "Mark read"}
                                  </button>
                                )}
                                <button
                                  className="secondary-button"
                                  onClick={() => handleDeleteNotification(notification)}
                                  type="button"
                                  disabled={notificationActionKey === `${notificationId}:read` || notificationActionKey === `${notificationId}:delete`}
                                >
                                  {notificationActionKey === `${notificationId}:delete` ? "Deleting..." : "Delete"}
                                </button>
                              </div>
                            </div>
                          );
                        })}
                      </div>
                    )}
                  </>
                )}
              </div>
            </section>
          </section>
        )}

        {activeTab === "sharing" && (
          <section className="content-grid">
            <section className="sharing-panel">
              <div className="panel">
                <div className="notifications-header">
                  <h2>My Shared Links</h2>
                  <button className="secondary-button" onClick={fetchSharedLinks} type="button" disabled={loadingSharedLinks}>
                    {loadingSharedLinks ? "Refreshing..." : "Refresh"}
                  </button>
                </div>

                {loadingSharedLinks ? (
                  <p className="loading-text">Loading shared links...</p>
                ) : sharedLinks.length === 0 ? (
                  <p className="empty-text">No shared links yet. Share a file or folder from My Files.</p>
                ) : (
                  <div className="shared-link-list">
                    {sharedLinks.map((link) => {
                      const sharedLinkId = getSharedLinkId(link);
                      const isEditing = editingSharedLinkId === sharedLinkId;
                      const shareUrl = getShareUrl(apiUrl, link);
                      const isActive = getBooleanValue(link, "isActive", "IsActive");

                      return (
                        <article className="shared-link-card" key={sharedLinkId}>
                          <div className="shared-link-card-header">
                            <div>
                              <strong>{getShareTargetTypeLabel(link)} Link</strong>
                              <small>{getSharePermissionLabel(link)} access</small>
                            </div>
                            <span className={`plan-state ${isActive ? "active" : "inactive"}`}>
                              {isActive ? "Active" : "Inactive"}
                            </span>
                          </div>

                          <div className="shared-link-url-row">
                            <input readOnly value={shareUrl} />
                            <button className="secondary-button" onClick={() => handleCopyShareUrl(link)} type="button">
                              Copy
                            </button>
                          </div>

                          {isEditing ? (
                            <div className="shared-link-edit-grid">
                              <label className="toggle-row">
                                <input
                                  type="checkbox"
                                  checked={editingSharedLinkForm.canView}
                                  onChange={(event) =>
                                    setEditingSharedLinkForm((current) => ({
                                      ...current,
                                      canView: event.target.checked
                                    }))
                                  }
                                />
                                Can view
                              </label>
                              <label className="toggle-row">
                                <input
                                  type="checkbox"
                                  checked={editingSharedLinkForm.allowDownload}
                                  onChange={(event) =>
                                    setEditingSharedLinkForm((current) => ({
                                      ...current,
                                      allowDownload: event.target.checked
                                    }))
                                  }
                                />
                                Allow download
                              </label>
                              <label className="toggle-row">
                                <input
                                  type="checkbox"
                                  checked={editingSharedLinkForm.canEdit}
                                  onChange={(event) =>
                                    setEditingSharedLinkForm((current) => ({
                                      ...current,
                                      canEdit: event.target.checked
                                    }))
                                  }
                                />
                                Can edit
                              </label>
                              <div className="shared-link-edit-actions">
                                <button
                                  className="primary-button"
                                  onClick={() => handleUpdateSharedLink(sharedLinkId)}
                                  type="button"
                                  disabled={shareActionKey === `${sharedLinkId}:update`}
                                >
                                  {shareActionKey === `${sharedLinkId}:update` ? "Saving..." : "Save"}
                                </button>
                                <button className="secondary-button" onClick={cancelEditSharedLink} type="button">
                                  Cancel
                                </button>
                              </div>
                            </div>
                          ) : (
                            <div className="shared-link-meta">
                              <span>Expires: {formatDate(getValue(link, "expirationDate", "ExpirationDate")) || "No expiry"}</span>
                              <span>Created: {formatDate(getValue(link, "createdAt", "CreatedAt"))}</span>
                            </div>
                          )}

                          {!isEditing && (
                            <div className="shared-link-actions">
                              <button className="secondary-button" onClick={() => startEditSharedLink(link)} type="button">
                                Edit
                              </button>
                              <button
                                className="secondary-button"
                                onClick={() => handleDeactivateSharedLink(sharedLinkId)}
                                type="button"
                                disabled={!isActive || shareActionKey === `${sharedLinkId}:deactivate`}
                              >
                                {shareActionKey === `${sharedLinkId}:deactivate` ? "Deactivating..." : "Deactivate"}
                              </button>
                            </div>
                          )}
                        </article>
                      );
                    })}
                  </div>
                )}
              </div>
            </section>
          </section>
        )}

        {activeTab === "subscription" && (
          <section className="content-grid">
            <section className="subscription-panel">
              <div className="panel">
                <h2>Subscription</h2>
                {loadingSubscription ? (
                  <p className="loading-text">Loading subscription...</p>
                ) : (
                  <>
                    {subscriptionError && (
                      <p className="inline-error">{subscriptionError}</p>
                    )}

                    {checkoutMessage && (
                      <div className="purchase-result-box">
                        <strong>Stripe checkout</strong>
                        <span>{checkoutMessage}</span>
                      </div>
                    )}

                    <div className="subscription-info">
                      <div className="subscription-row">
                        <span>Current plan</span>
                        <strong>{latestSubscription ? getValue(latestSubscription, "planName", "PlanName") : "No subscription yet"}</strong>
                      </div>
                      <div className="subscription-row">
                        <span>Status</span>
                        <strong className={`status-${getSubscriptionStatusLabel(latestSubscription).toLowerCase().replace(/\s/g, "-")}`}>
                          {latestSubscription ? getSubscriptionStatusLabel(latestSubscription) : "Not subscribed"}
                        </strong>
                      </div>
                      <div className="subscription-row">
                        <span>Current period</span>
                        <span>
                          {latestSubscription
                            ? `${formatDate(getValue(latestSubscription, "currentPeriodStart", "CurrentPeriodStart"))} - ${formatDate(getValue(latestSubscription, "currentPeriodEnd", "CurrentPeriodEnd"))}`
                            : "No billing period yet"}
                        </span>
                      </div>
                      <div className="subscription-row">
                        <span>Storage limit</span>
                        <span>{formatBytes(entitlements?.StorageLimitBytes || 0)}</span>
                      </div>
                    </div>

                    <section className="subscription-plan-list">
                      <div className="subscriptions-admin-header">
                        <div>
                          <p className="eyebrow">Plans</p>
                          <h3>Choose a subscription plan</h3>
                          <p>Select one of the plans created in the admin dashboard and continue to Stripe checkout.</p>
                        </div>
                      </div>

                      {subscriptionPlans.length === 0 ? (
                        <p className="empty-text">No active subscription plans are available yet.</p>
                      ) : (
                        <div className="subscription-plan-grid">
                          {subscriptionPlans.map((plan) => {
                            const planId = getValue(plan, "subscriptionPlanId", "SubscriptionPlanId");
                            const currentPlanId = latestSubscription ? getValue(latestSubscription, "subscriptionPlanId", "SubscriptionPlanId") : "";
                            const currentStatus = latestSubscription ? getSubscriptionStatusValue(latestSubscription) : "";
                            const isCurrentPlan = currentPlanId === planId && (currentStatus === "0" || currentStatus === "1");
                            const isPendingPlan = currentPlanId === planId && currentStatus === "5";

                            return (
                              <article className="subscription-plan-card" key={planId}>
                                <div>
                                  <strong>{getValue(plan, "name", "Name")}</strong>
                                  <p>{getValue(plan, "description", "Description") || "No description provided."}</p>
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
                                    <dt>Max file size</dt>
                                    <dd>{formatBytes(Number(getValue(plan, "maxFileSizeBytes", "MaxFileSizeBytes")) || 0)}</dd>
                                  </div>
                                </dl>

                                <div className="subscription-plan-card-footer">
                                  <span className={`plan-state ${isCurrentPlan ? "active" : isPendingPlan ? "inactive" : "active"}`}>
                                    {isCurrentPlan ? "Current Plan" : isPendingPlan ? "Payment Pending" : "Available"}
                                  </span>
                                  <button
                                    className="primary-button"
                                    type="button"
                                    onClick={() => handleSubscribeToPlan(plan)}
                                    disabled={!!subscriptionActionKey || isCurrentPlan || isPendingPlan}
                                  >
                                    {subscriptionActionKey === planId
                                      ? "Redirecting..."
                                      : isCurrentPlan
                                        ? "Current Plan"
                                        : isPendingPlan
                                          ? "Pending"
                                          : "Choose Plan"}
                                  </button>
                                </div>
                              </article>
                            );
                          })}
                        </div>
                      )}
                    </section>
                  </>
                )}
              </div>
            </section>
          </section>
        )}

        {shareTarget && (
          <div className="share-dialog-backdrop" onClick={closeShareDialog}>
            <div className="share-dialog" onClick={(event) => event.stopPropagation()}>
              <div className="share-dialog-header">
                <div>
                  <p className="eyebrow">Link Sharing</p>
                  <h2>Share {shareTarget.name}</h2>
                </div>
                <button className="secondary-button" onClick={closeShareDialog} type="button">
                  Close
                </button>
              </div>

              <form className="share-dialog-form" onSubmit={handleCreateShareLink}>
                <label>
                  Permission
                  <select
                    value={shareForm.permission}
                    onChange={(event) => setShareForm((current) => ({ ...current, permission: event.target.value }))}
                  >
                    {sharePermissionOptions.map((option) => (
                      <option key={option.value} value={option.value}>{option.label}</option>
                    ))}
                  </select>
                </label>
                <button
                  className="primary-button"
                  disabled={shareActionKey === `${shareTarget.targetType}:${shareTarget.itemId}:share`}
                  type="submit"
                >
                  {shareActionKey === `${shareTarget.targetType}:${shareTarget.itemId}:share` ? "Creating..." : "Create Link"}
                </button>
              </form>

              {shareResult && (
                <div className="purchase-result-box">
                  <strong>Share link created</strong>
                  <a href={getShareUrl(apiUrl, shareResult)} target="_blank" rel="noreferrer">
                    Open shared link
                  </a>
                  <span>{getShareUrl(apiUrl, shareResult)}</span>
                </div>
              )}
            </div>
          </div>
        )}
      </section>
    </main>
  );
}

export default UserHomePage;
