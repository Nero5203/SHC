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

export function getUserIdFromToken() {
  const token = getAuthToken();
  if (!token) return null;

  try {
    const payload = token.split(".")[1];
    const decoded = JSON.parse(atob(payload.replace(/-/g, "+").replace(/_/g, "/")));
    return decoded.nameid
      || decoded.sub
      || decoded[ClaimTypes.NameIdentifier]
      || decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
  } catch (error) {
    return null;
  }
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
  Name: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
};

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
