export interface loginInput {
    emailorPhoneNumber: string;
    password: string;
}

export interface loginOutput {
    token: string;
    ipAnalysis?: any;
    email: string;
    fullName: string;
    firstName: string;
    lastName: string;
    userId: string;
    code?: any;
    isRequiresTwoFactor: boolean;
    emailStar?: any;
    phoneStar?: any;
  }