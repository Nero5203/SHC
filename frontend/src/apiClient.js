export const defaultApiUrl = "http://127.0.0.1:5165";

export async function postJson(apiUrl, path, body) {
  const response = await fetch(`${apiUrl.replace(/\/$/, "")}${path}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(body)
  });

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
