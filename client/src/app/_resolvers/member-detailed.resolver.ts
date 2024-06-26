import { ResolveFn } from '@angular/router';
import { Member } from '../_models/member';
import { inject } from '@angular/core';
import { MembersService } from '../_services/members.service';

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
  const memeberservice=inject(MembersService);
  return memeberservice.getMember(route.paramMap.get('username')!);
};
