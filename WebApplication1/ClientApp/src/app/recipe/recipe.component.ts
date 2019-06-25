import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-recipe-component',
  templateUrl: './recipe.component.html'
})
export class RecipeComponent {
  public recipe: Recipe;

  constructor(
    http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    route: ActivatedRoute
  )
  {
    route.params.subscribe(params => {
      http.get<Recipe>(baseUrl + 'api/SampleData/WeatherForecasts/' + params['id']).subscribe(result => {
        this.recipe = result;
      }, error => console.error(error));
    });
  }
}

interface Recipe {
  name: string;
  sourceUrl: string;
  ingredients: Ingredient[];
  description: string;
}

interface Ingredient {
  ingredient: OneIngredient;
  amount: string;
  measureUnit: MeasureInit;
}

interface OneIngredient {
  name: string;
}

interface MeasureInit {
  fullName: string;
}
