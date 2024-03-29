export enum UserRole {
  Unauthenticated,
  Registered,
  Company_Admin,
  System_Admin
}

export interface User {
  id: number;
  email: string;
  password?: string;
  confirmPassword?: string,
  name: string;
  surname: string;
  city?: string;
  country?: string;
  phone?: string;
  job?: string;
  companyId?: number;
  userRole?: UserRole;
  companyInfo?: string;
  isFirstLogin?: string;
  penalPoints?: number;
}
