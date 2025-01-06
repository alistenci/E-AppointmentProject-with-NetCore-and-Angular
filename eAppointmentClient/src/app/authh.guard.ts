import { CanActivateChildFn } from '@angular/router';

export const authhGuard: CanActivateChildFn = (childRoute, state) => {
  return true;
};
