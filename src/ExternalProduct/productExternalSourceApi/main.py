from fastapi import FastAPI, Query, HTTPException
from typing import Dict, List, Optional
from services.edamam_recipe_api import (
    fetch_all_recipes_from_edamam,
    fetch_recipe_from_edamam,
    search_recipes,
)
from services.open_food_facts_api import search_open_food_facts, fetch_open_food_facts
from services.usda_api import search_usda_food, fetch_usda_food
from models import FoodWithRecipe, StandardFood
from logger import logger, _

app = FastAPI()


@app.get("/api/search", response_model=tuple[List[StandardFood], List[int]])
async def search_food(
    q: str,
    page: int = 1,
    source: Optional[str] = None,
    min_calories: Optional[float] = 0,
    max_calories: Optional[float] = Query(float("inf")),
):
    logger.info(
        "Search request received 2",
        extra={
            "query": q,
            "source": source,
            "minCalories": min_calories,
            "maxCalories": max_calories,
        },
    )
    try:
        if source == "usda":
            results = search_usda_food(q, page)
            results = (results[0], [results[1]])
        elif source == "openfoodfacts":
            results = search_open_food_facts(q, page)
            results = (results[0], [results[1]])
        else:
            results = (
                search_usda_food(q, page)[0] + search_open_food_facts(q, page)[0],
                [search_usda_food(q, page)[1], search_open_food_facts(q, page)[1]],
            )

        logger.info(_("Results received, {results}", results=results))

        return results
    except Exception as e:
        logger.error(_("Failed to fetch data, {error}", error=e))
        raise HTTPException(status_code=500, detail="Failed to fetch data")


@app.get("/api", response_model=StandardFood)
async def get_food(
    id: str,
    source: str,
):
    logger.info(
        _(
            "Getting by {id} source {source}",
            id=id,
            source=source,
        )
    )
    try:
        if source == "usda":
            result = fetch_usda_food(id)
        elif source == "openfoodfacts":
            result = fetch_open_food_facts(id)
        elif source == "edamam_recipe":
            result = fetch_recipe_from_edamam([id])

        logger.info(
            _(
                "Getting by {id} source {source} processed success",
                id=id,
                source=source,
            )
        )
        return result
    except Exception as e:
        logger.error(_("Failed to fetch data, {error}", error=str(e)))
        raise HTTPException(status_code=500, detail="Failed to fetch data")


@app.get("/api/recipe/s", response_model=tuple[List[FoodWithRecipe], str])
async def search_edamam_recipes(
    q: str, cont: str | None = None
) -> tuple[List[FoodWithRecipe], str]:
    return search_recipes(q, cont)


@app.get("/api/by-uri", response_model=List[FoodWithRecipe])
async def get_food_from_edamam(
    uri: List[str] = Query(),
):
    logger.info(
        _(
            "Getting by uri {id}",
            id=id,
        )
    )
    try:
        result = await fetch_all_recipes_from_edamam(uri)

        logger.info(
            _(
                "Received by {id}",
                id=id,
            )
        )
        return result
    except Exception as e:
        logger.error(_("Failed to fetch data, {error}", error=str(e)))
        raise HTTPException(status_code=500, detail="Failed to fetch data")


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(
        app,
        host="0.0.0.0",
        port=8080,
        ssl_keyfile="./certs/dt.key",
        ssl_certfile="./certs/dt.crt",
    )
