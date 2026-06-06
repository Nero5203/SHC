export const defaultApiUrl = "http://127.0.0.1:5165";

export async function postJson(apiUrl, path, body) {
  const token = getAuthToken();
  const response = await fetch(`${apiUrl.replace(/\/$/, "")}${path}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    },
    body: JSON.stringify(body)
  });

  return handleResponse(response);
}

export async function getJson(apiUrl, path) {
  const token = getAuthToken();
  const response = await fetch(`${apiUrl.replace(/\/$/, "")}${path}`, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });

  return handleResponse(response);
}

export async function putJson(apiUrl, path, body) {
  const token = getAuthToken();
  const response = await fetch(`${apiUrl.replace(/\/$/, "")}${path}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    },
    body: JSON.stringify(body)
  });

  return handleResponse(response);
}

export async function deleteJson(apiUrl, path) {
  const token = getAuthToken();
  const response = await fetch(`${apiUrl.replace(/\/$/, "")}${path}`, {
    method: "DELETE",
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });

  return handleResponse(response);
}

export function getAuthToken() {
  return localStorage.getItem("shc.authToken");
}

export function getRefreshToken() {
  return localStorage.getItem("shc.refreshToken");
}

export function setAuthToken(token) {
  if (token) {
    localStorage.setItem("shc.authToken", token);
  } else {
    localStorage.removeItem("shc.authToken");
  }
}

export function setRefreshToken(token) {
  if (token) {
    localStorage.setItem("shc.refreshToken", token);
  } else {
    localStorage.removeItem("shc.refreshToken");
  }
}

export function clearAuth() {
  localStorage.removeItem("shc.authToken");
  localStorage.removeItem("shc.refreshToken");
}

export function isAuthenticated() {
  return !!getAuthToken();
}

export function decodeTokenPayload(token = getAuthToken()) {
  if (!token) return null;

  try {
    const payload = token.split(".")[1];
    const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");
    const padded = base64.padEnd(base64.length + ((4 - base64.length % 4) % 4), "=");
    return JSON.parse(atob(padded));
  } catch (error) {
    return null;
  }
}

export function getUserIdFromToken(token = getAuthToken()) {
  const decoded = decodeTokenPayload(token);
  if (!decoded) return null;

  return decoded.nameid
    || decoded.sub
    || decoded[ClaimTypes.NameIdentifier];
}

export function getUserNameFromToken(token = getAuthToken()) {
  const decoded = decodeTokenPayload(token);
  if (!decoded) return "";

  return decoded[ClaimTypes.Name]
    || decoded.name
    || decoded.unique_name
    || decoded[ClaimTypes.Email]
    || decoded.email
    || "";
}

export function getRolesFromToken(token = getAuthToken()) {
  return getClaimValues(decodeTokenPayload(token), ClaimTypes.Role, "role", "roles");
}

export function getPermissionsFromToken(token = getAuthToken()) {
  return getClaimValues(decodeTokenPayload(token), "permission", "permissions");
}

export function hasRole(role, token = getAuthToken()) {
  return getRolesFromToken(token).some((value) => sameText(value, role));
}

export function hasPermission(permission, token = getAuthToken()) {
  return getPermissionsFromToken(token).some((value) => sameText(value, permission));
}

export function isAdminToken(token = getAuthToken()) {
  return hasRole("Admin", token) || hasPermission("System.Admin", token);
}

export function getDashboardPageFromToken(token = getAuthToken()) {
  return isAdminToken(token) ? "admin" : "home";
}

export function extractTokens(loginResponse) {
  const tokenPayload = loginResponse?.token ?? loginResponse?.Token ?? loginResponse;

  const accessToken = tokenPayload?.accessToken
    ?? tokenPayload?.AccessToken
    ?? tokenPayload?.acessToken
    ?? tokenPayload?.AcessToken
    ?? (typeof tokenPayload === "string" ? tokenPayload : "");

  const refreshToken = tokenPayload?.refreshToken
    ?? tokenPayload?.RefreshToken
    ?? loginResponse?.refreshToken
    ?? loginResponse?.RefreshToken
    ?? "";

  return { accessToken, refreshToken };
}

export const ClaimTypes = {
  NameIdentifier: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
  Email: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
  Name: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
  Role: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
};

function getClaimValues(payload, ...claimNames) {
  if (!payload) return [];

  return claimNames
    .flatMap((claimName) => normalizeClaimValue(payload[claimName]))
    .filter((value) => typeof value === "string" && value.trim())
    .map((value) => value.trim());
}

function normalizeClaimValue(value) {
  if (Array.isArray(value)) return value;
  if (value === undefined || value === null) return [];
  return [value];
}

function sameText(left, right) {
  return String(left).toLowerCase() === String(right).toLowerCase();
}

async function handleResponse(response) {
  const contentType = response.headers.get("content-type") ?? "";
  const payload = contentType.includes("application/json")
    ? await response.json()
    : await response.text();

  if (!response.ok) {
    const message = typeof payload === "string" && payload
      ? payload
      : payload?.title ?? payload?.message ?? `Request failed with ${response.status}`;
    throw new Error(message);
  }

  return payload;
}
