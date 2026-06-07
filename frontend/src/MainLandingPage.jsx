const highlights = [
  {
    title: "Team file storage",
    text: "Store files in folders, keep versions organized, and route uploads to your storage nodes with room-aware allocation."
  },
  {
    title: "Secure link sharing",
    text: "Create share links, control expiration, and keep file delivery fast without sending large attachments around."
  },
  {
    title: "Admin oversight",
    text: "Manage users, plans, purchases, storage nodes, and system settings from one backend-connected dashboard."
  },
  {
    title: "AI workspace help",
    text: "Surface file insights and suggestions from real activity so the product can guide users back to active work."
  }
];

const modules = [
  "Users and roles",
  "File storage",
  "Link sharing",
  "Storage nodes",
  "System settings",
  "Subscriptions and purchases"
];

const stats = [
  { value: "3-node ready", label: "Built around your shared storage-node idea" },
  { value: "Role-based", label: "Separate admin and user dashboards after login" },
  { value: "Backend-first", label: "Connected to your API flow for real auth and data" }
];

function MainLandingPage({ onNavigate }) {
  return (
    <main className="landing-page">
      <section className="landing-hero">
        <header className="landing-nav">
          <div className="landing-brand">
            <div className="landing-brand-mark">S</div>
            <div>
              <strong>SHC</strong>
              <span>Shared Hybrid Cloud</span>
            </div>
          </div>

          <nav className="landing-actions" aria-label="Main navigation">
            <button className="nav-text-button" type="button" onClick={() => onNavigate("login")}>
              Sign in
            </button>
            <button className="primary-button hero-cta-button" type="button" onClick={() => onNavigate("register")}>
              Start with your team
            </button>
          </nav>
        </header>

        <div className="landing-hero-content">
          <div className="landing-copy">
            <p className="landing-kicker">Shared storage for real teams</p>
            <h1>Bring your files, storage nodes, and team workflows together.</h1>
            <p className="landing-summary">
              SHC gives your team one place to upload, organize, share, and manage content across your own storage-node setup,
              with separate spaces for users, admins, subscriptions, and system control.
            </p>

            <div className="landing-cta-row">
              <button className="primary-button landing-primary" type="button" onClick={() => onNavigate("register")}>
                Create account
              </button>
              <button className="secondary-button landing-secondary" type="button" onClick={() => onNavigate("login")}>
                Open sign in
              </button>
            </div>

            <ul className="landing-module-strip" aria-label="Platform modules">
              {modules.map((module) => (
                <li key={module}>{module}</li>
              ))}
            </ul>
          </div>

          <div className="landing-visual" aria-hidden="true">
            <div className="workspace-scene">
              <div className="workspace-screen workspace-screen-main">
                <div className="screen-topbar">
                  <span />
                  <span />
                  <span />
                </div>

                <div className="screen-body">
                  <div className="screen-sidebar">
                    <div className="sidebar-chip active">Storage</div>
                    <div className="sidebar-chip">Shared links</div>
                    <div className="sidebar-chip">AI</div>
                    <div className="sidebar-chip">Billing</div>
                  </div>

                  <div className="screen-panel">
                    <div className="panel-header-row">
                      <div>
                        <strong>Project workspace</strong>
                        <small>Uploads, folders, shares</small>
                      </div>
                      <div className="storage-pill">Node online</div>
                    </div>

                    <div className="file-list">
                      <div className="file-row">
                        <span className="file-dot blue" />
                        <strong>Campaign-Brief.pdf</strong>
                        <small>Shared with team</small>
                      </div>
                      <div className="file-row">
                        <span className="file-dot coral" />
                        <strong>Q3-Launch</strong>
                        <small>Folder</small>
                      </div>
                      <div className="file-row">
                        <span className="file-dot green" />
                        <strong>Budget-v4.xlsx</strong>
                        <small>Protected</small>
                      </div>
                    </div>

                    <div className="capacity-block">
                      <div className="capacity-copy">
                        <strong>Cluster capacity</strong>
                        <small>3 laptops acting as storage nodes</small>
                      </div>
                      <div className="capacity-bar">
                        <span className="capacity-fill" />
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div className="workspace-screen workspace-screen-side">
                <strong>Activity</strong>
                <div className="activity-item">AI suggestion created</div>
                <div className="activity-item">Shared link expires in 7 days</div>
                <div className="activity-item">Purchase recorded for Standard plan</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="landing-proof">
        <div className="landing-proof-inner">
          {stats.map((stat) => (
            <div key={stat.label} className="proof-stat">
              <strong>{stat.value}</strong>
              <span>{stat.label}</span>
            </div>
          ))}
        </div>
      </section>

      <section className="landing-section landing-section-light">
        <div className="section-heading">
          <p className="eyebrow">Why teams use it</p>
          <h2>Built around the modules your project already has.</h2>
          <p>
            Instead of a fake marketing shell, this homepage leads into the actual flows you already wired:
            registration, login, admin dashboards, file storage, purchases, and sharing.
          </p>
        </div>

        <div className="landing-feature-grid">
          {highlights.map((item) => (
            <article key={item.title} className="landing-feature">
              <h3>{item.title}</h3>
              <p>{item.text}</p>
            </article>
          ))}
        </div>
      </section>

      <section className="landing-section landing-section-dark">
        <div className="workflow-band">
          <div className="workflow-copy">
            <p className="eyebrow">For admins</p>
            <h2>See your cloud platform as one operating surface.</h2>
            <p>
              Track users, assign roles, review purchases, manage subscriptions, monitor storage-node health,
              and adjust system settings without hopping between disconnected tools.
            </p>
          </div>

          <div className="workflow-preview">
            <div className="workflow-preview-top">
              <strong>Admin overview</strong>
              <span>Live system summary</span>
            </div>
            <div className="workflow-metrics">
              <div>
                <strong>124</strong>
                <span>Active users</span>
              </div>
              <div>
                <strong>3/3</strong>
                <span>Nodes online</span>
              </div>
              <div>
                <strong>18</strong>
                <span>Pending shares</span>
              </div>
            </div>
          </div>
        </div>

        <div className="workflow-band workflow-band-reverse">
          <div className="workflow-preview">
            <div className="workflow-preview-top">
              <strong>User workspace</strong>
              <span>Storage, folders, shares</span>
            </div>
            <div className="workflow-lines">
              <span>Upload files to a folder</span>
              <span>Create expiring share links</span>
              <span>See AI suggestions from file activity</span>
            </div>
          </div>

          <div className="workflow-copy">
            <p className="eyebrow">For users</p>
            <h2>Move from upload to sharing without leaving the workspace.</h2>
            <p>
              Users can organize files, download what they need, share links, and manage their account
              while the backend keeps ownership, storage-node assignment, and billing relations in sync.
            </p>
          </div>
        </div>
      </section>

      <section className="landing-footer-cta">
        <div className="landing-footer-copy">
          <h2>Start with the public homepage, then flow into the real product.</h2>
          <p>
            Open registration for new teammates or sign in and continue directly into the dashboard your role allows.
          </p>
        </div>

        <div className="landing-cta-row">
          <button className="primary-button landing-primary" type="button" onClick={() => onNavigate("register")}>
            Register a user
          </button>
          <button className="secondary-button landing-secondary" type="button" onClick={() => onNavigate("login")}>
            Sign in
          </button>
        </div>
      </section>
    </main>
  );
}

export default MainLandingPage;
