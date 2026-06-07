function Icon({ label, children, size = 18 }) {
  return (
    <span
      aria-hidden="true"
      className="ui-icon"
      style={{ width: size, height: size, fontSize: Math.max(11, Math.round(size * 0.6)) }}
      title={label}
    >
      {children}
    </span>
  );
}

export function ArrowLeft(props) {
  return <Icon label="Back" {...props}>←</Icon>;
}

export function ArrowRight(props) {
  return <Icon label="Next" {...props}>→</Icon>;
}

export function Bell(props) {
  return <Icon label="Notifications" {...props}>B</Icon>;
}

export function Bot(props) {
  return <Icon label="AI" {...props}>AI</Icon>;
}

export function ChevronRight(props) {
  return <Icon label="Open" {...props}>›</Icon>;
}

export function CreditCard(props) {
  return <Icon label="Billing" {...props}>$</Icon>;
}

export function FolderOpen(props) {
  return <Icon label="Folder" {...props}>F</Icon>;
}

export function LayoutDashboard(props) {
  return <Icon label="Dashboard" {...props}>D</Icon>;
}

export function Link2(props) {
  return <Icon label="Link" {...props}>L</Icon>;
}

export function Lock(props) {
  return <Icon label="Lock" {...props}>O</Icon>;
}

export function LogIn(props) {
  return <Icon label="Login" {...props}>I</Icon>;
}

export function LogOut(props) {
  return <Icon label="Logout" {...props}>O</Icon>;
}

export function Mail(props) {
  return <Icon label="Mail" {...props}>@</Icon>;
}

export function Phone(props) {
  return <Icon label="Phone" {...props}>P</Icon>;
}

export function Plus(props) {
  return <Icon label="Add" {...props}>+</Icon>;
}

export function Search(props) {
  return <Icon label="Search" {...props}>S</Icon>;
}

export function Server(props) {
  return <Icon label="Server" {...props}>N</Icon>;
}

export function Settings(props) {
  return <Icon label="Settings" {...props}>S</Icon>;
}

export function Shield(props) {
  return <Icon label="Security" {...props}>H</Icon>;
}

export function ShieldCheck(props) {
  return <Icon label="Verified" {...props}>C</Icon>;
}

export function ShoppingCart(props) {
  return <Icon label="Purchases" {...props}>C</Icon>;
}

export function Sparkles(props) {
  return <Icon label="Sparkles" {...props}>A</Icon>;
}

export function UserCircle2(props) {
  return <Icon label="User" {...props}>U</Icon>;
}

export function UserPlus(props) {
  return <Icon label="Register" {...props}>+</Icon>;
}

export function Users(props) {
  return <Icon label="Users" {...props}>U</Icon>;
}

export function Upload(props) {
  return <Icon label="Upload" {...props}>↑</Icon>;
}
