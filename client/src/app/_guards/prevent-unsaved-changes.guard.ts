import { Injectable } from '@angular/core';
import { CanDeactivate} from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  canDeactivate(component: MemberEditComponent): boolean  {
    if (component.editForm.dirty) {
      return confirm("Are you sure you want to continue? Any Unsaved changes will be lost!");
    }
    return true;
  }
  
}
