import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public week: Week;

  constructor(
    http: HttpClient,
    @Inject('BASE_URL') baseUrl: string
  ) {
      http.get<Week>(baseUrl + 'api/SampleData/CurrentWeek/currentweek').subscribe(result => {
        this.week = result;
      }, error => console.error(error));
  }
}

interface Week {
  mondayId: string;
  monday: Day;
  tuesday: Day;
  wednesday: Day;
  thursday: Day;
  friday: Day;
  saturday: Day;
  sunday: Day;
}

interface Day {
  dinner: Dinner;
}

interface Dinner {
  id: string;
  name: string;
}
