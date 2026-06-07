import {
  ArrowRight,
  Bot,
  FolderOpen,
  Link2,
  Lock,
  Server,
  ShieldCheck
} from "./icons.jsx";

const highlights = [
  {
    title: "Secure storage",
    text: "Upload to your own storage-node network, keep folders organized, and monitor capacity from one place.",
    icon: ShieldCheck
  },
  {
    title: "Link sharing",
    text: "Share files and folders with expiration-aware links and download controls already backed by your API.",
    icon: Link2
  },
  {
    title: "AI suggestions",
    text: "Surface useful cleanup, organization, and activity suggestions directly from user file behavior.",
    icon: Bot
  },
  {
    title: "Storage nodes",
    text: "Run SHC DRIVE on your own machines and keep the infrastructure visible to admins in real time.",
    icon: Server
  }
];

const quickStats = [
  { value: "Role-aware", label: "Separate user and admin workspaces after login" },
  { value: "Real API", label: "Auth, files, sharing, billing, and nodes already wired" },
  { value: "Node-ready", label: "Fits your 3-laptop storage-node model from day one" }
];

function MainLandingPage({ onNavigate }) {
  return (
    <main className="landing-page">
      <section className="landing-hero">
        <header className="landing-nav">
          <div className="landing-brand">
            <div className="landing-brand-mark" aria-hidden="true" />
            <div>
              <strong>SHC DRIVE</strong>
              <span>Secure cloud workspace</span>
            </div>
          </div>

          <nav className="landing-actions" aria-label="Main navigation">
            <button className="nav-text-button" type="button" onClick={() => document.getElementById("landing-features")?.scrollIntoView({ behavior: "smooth" })}>
              Features
            </button>
            <button className="nav-text-button" type="button" onClick={() => document.getElementById("landing-pricing")?.scrollIntoView({ behavior: "smooth" })}>
              Pricing
            </button>
            <button className="secondary-button hero-cta-button" type="button" onClick={() => onNavigate("login")}>
              Login
            </button>
          </nav>
        </header>

        <div className="landing-hero-content">
          <div className="landing-copy">
            <p className="landing-kicker">
              <Lock size={14} />
              Secure. Smart. Simple.
            </p>
            <h1>
              Store, share, and manage your files
              <span> securely</span>
            </h1>
            <p className="landing-summary">
              SHC DRIVE gives your team one place to upload, organize, share, and manage content across your own storage-node setup,
              with user dashboards, admin controls, subscriptions, notifications, and AI suggestions already connected to the backend.
            </p>

            <div className="landing-cta-row">
              <button className="primary-button landing-primary" type="button" onClick={() => onNavigate("register")}>
                Get Started Free
              </button>
              <button className="landing-link-button" type="button" onClick={() => onNavigate("login")}>
                See how it works
                <ArrowRight size={16} />
              </button>
            </div>

            <div className="landing-workspace-preview">
              <div className="landing-workspace-sidebar">
                <div className="landing-workspace-logo">
                  <div className="landing-brand-mark small" aria-hidden="true" />
                  <span>SHC DRIVE</span>
                </div>
                <button className="landing-mini-nav active" type="button">My Files</button>
                <button className="landing-mini-nav" type="button">Shared</button>
                <button className="landing-mini-nav" type="button">Recent</button>
                <button className="landing-mini-nav" type="button">Trash</button>
              </div>

              <div className="landing-workspace-main">
                <div className="landing-workspace-folders">
                  <div className="landing-folder-card">
                    <FolderOpen size={22} />
                    <span>Projects</span>
                  </div>
                  <div className="landing-folder-card">
                    <FolderOpen size={22} />
                    <span>Documents</span>
                  </div>
                  <div className="landing-folder-card">
                    <FolderOpen size={22} />
                    <span>Photos</span>
                  </div>
                </div>

                <div className="landing-workspace-files">
                  <div className="landing-file-tile pdf">PDF</div>
                  <div className="landing-file-tile sheet">XLS</div>
                  <div className="landing-file-tile doc">DOC</div>
                  <div className="landing-file-tile upload">+</div>
                </div>
              </div>
            </div>
          </div>

          <div className="landing-auth-card">
            <div className="landing-auth-icon">
              <Lock size={28} />
            </div>
            <h2>Welcome back</h2>
            <p>Login to access your SHC DRIVE account.</p>

            <div className="landing-auth-fields">
              <label>
                <span>Email</span>
                <div className="landing-auth-input">you@example.com</div>
              </label>
              <label>
                <span>Password</span>
                <div className="landing-auth-input">Enter your password</div>
              </label>
            </div>

            <div className="landing-auth-row">
              <span>Remember me</span>
              <button className="landing-text-link" type="button" onClick={() => onNavigate("login")}>
                Open login
              </button>
            </div>

            <button className="primary-button landing-auth-submit" type="button" onClick={() => onNavigate("login")}>
              Login
            </button>

            <div className="landing-auth-divider">
              <span />
              <span>or</span>
              <span />
            </div>

            <p className="landing-auth-footer">
              Don&apos;t have an account?
              <button className="landing-text-link" type="button" onClick={() => onNavigate("register")}>
                Create an account
              </button>
            </p>
          </div>
        </div>
      </section>

      <section className="landing-proof">
        <div className="landing-proof-inner">
          {quickStats.map((stat) => (
            <div key={stat.label} className="proof-stat">
              <strong>{stat.value}</strong>
              <span>{stat.label}</span>
            </div>
          ))}
        </div>
      </section>

      <section className="landing-section landing-section-light" id="landing-features">
        <div className="section-heading">
          <p className="eyebrow">Why teams use it</p>
          <h2>Built around the backend modules you already have.</h2>
          <p>
            This is not a fake marketing shell. It leads into the real flows you already wired:
            registration, login, dashboards, file storage, sharing, subscriptions, purchases, and infrastructure management.
          </p>
        </div>

        <div className="landing-feature-grid">
          {highlights.map((item) => (
            <article key={item.title} className="landing-feature">
              <div className="landing-feature-icon">
                <item.icon size={20} />
              </div>
              <h3>{item.title}</h3>
              <p>{item.text}</p>
            </article>
          ))}
        </div>
      </section>

      <section className="landing-section landing-section-dark" id="landing-pricing">
        <div className="workflow-band">
          <div className="workflow-copy">
            <p className="eyebrow">For admins</p>
            <h2>Run your cloud platform like one operating surface.</h2>
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
          <h2>Start with SHC DRIVE, then continue straight into the real product.</h2>
          <p>
            Register a teammate, sign in, and move directly into the dashboard your role is allowed to use.
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
